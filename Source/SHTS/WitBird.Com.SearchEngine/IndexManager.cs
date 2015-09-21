using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace WitBird.Com.SearchEngine
{
    public class IndexManager
    {
        public string IndexPath
        {
            set;
            get;
        }

        public IndexManager(string indexPath, string dictFileName)
        {
            this.IndexPath = indexPath;
            PanGu.Segment.Init(dictFileName); 
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="sourceList">数据源,将数据转换成为文档对象 </param>
        public void CreateIndexByData(List<MetaSource> sourceList)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(IndexPath), new NativeFSLockFactory());
            //IndexReader:对索引库进行读取的类
            //是否存在索引库文件夹以及索引库特征文件
            bool isExist = IndexReader.IndexExists(directory); 
            if (isExist)
            {
                //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }

            //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
            //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
            // 遍历数据源 将数据转换成为文档对象 存入索引库
            foreach (var source in sourceList)
            {
                //new一篇文档对象 --一条记录对应索引库中的一个文档
                Document document = new Document(); 
                //向文档中添加字段  Add(字段,值,是否保存字段原始值,是否针对该列创建索引)
                //--所有字段的值都将以字符串类型保存 因为索引库只存储字符串类型数据
                document.Add(new Field(Constants.TITILE, source.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));

                //Field.Store:表示是否保存字段原值。指定Field.Store.YES的字段在检索时才能用document.Get取出原值  
                //Field.Index.NOT_ANALYZED:指定不按照分词后的结果保存--是否按分词后结果保存取决于是否对该列内容进行模糊查询
                document.Add(new Field(Constants.URL, source.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));

                //Field.Index.ANALYZED:指定文章内容按照分词后结果保存 否则无法实现后续的模糊查询 
                //WITH_POSITIONS_OFFSETS:指示不仅保存分割后的词 还保存词之间的距离

                document.Add(new Field(Constants.TIME, source.Time, Field.Store.YES, Field.Index.NOT_ANALYZED));
                document.Add(new Field(Constants.IMGS, source.Imgs, Field.Store.YES, Field.Index.NOT_ANALYZED));
                document.Add(new Field(Constants.CONTENT, source.Content, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));

                //文档写入索引库
                writer.AddDocument(document); 
            }
            //会自动解锁
            writer.Dispose();
            //不要忘了Close，否则索引结果搜不到
            directory.Dispose();
            directory.Dispose();
        }

        /// <summary>
        /// 增加索引
        /// </summary>
        /// <param name="sourceList">数据源,将数据转换成为文档对象 </param>
        public void AddIndexByData(List<MetaSource> sourceList)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(IndexPath), new NativeFSLockFactory());
            IndexWriter writer = null;
            try
            {
                bool isExist = IndexReader.IndexExists(directory);
                if (isExist)
                {
                    if (IndexWriter.IsLocked(directory))
                    {
                        IndexWriter.Unlock(directory);
                    }
                }
                writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
                Document document = null;
                foreach (var source in sourceList)
                {
                    document = new Document();
                    document.Add(new Field(Constants.TITILE, source.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                    document.Add(new Field(Constants.URL, source.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field(Constants.TIME, source.Time, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field(Constants.CREATETIME, source.Time, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field(Constants.IMGS, source.Imgs, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field(Constants.CONTENT, source.Content, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                    //文档写入索引库
                    writer.AddDocument(document);
                }
            }
            finally
            {
                writer.Dispose();
                directory.Dispose();
            }
        }

        /// <summary>
        /// 从索引库中检索关键字
        /// </summary>
        /// <param name="sourceList">检索关键字 </param>
        public List<MetaSource> SearchFromIndexData(string kewWords)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(IndexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件
            PhraseQuery query = new PhraseQuery();
            //把用户输入的关键字进行分词
            foreach (string word in SplitContent.SplitWords(kewWords))
            {
                query.Add(new Term(Constants.CONTENT, word));
            }
            //query.Add(new Term("content", "C#"));//多个查询条件时 为且的关系
            //指定关键词相隔最大距离
            query.Slop=100; 

            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.Create(200, true);
            
            //根据query查询条件进行查询，查询结果放入collector容器
            searcher.Search(query, null, collector);
            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.TopDocs(0, collector.TotalHits).ScoreDocs;

            //展示数据实体对象集合
            List<MetaSource> searchResult = new List<MetaSource>();
            for (int i = 0; i < docs.Length; i++)
            {
                //得到查询结果文档的id（Lucene内部分配的id）
                int docId = docs[i].Doc;
                //根据文档id来获得文档对象Document
                Document doc = searcher.Doc(docId);
                MetaSource result = new MetaSource();
                result.Title = doc.Get(Constants.TITILE);
                //搜索关键字高亮显示 使用盘古提供高亮插件
                result.Content = SplitContent.HightLight(kewWords, doc.Get(Constants.CONTENT));
                result.Time = doc.Get(Constants.TIME);
                result.Imgs = doc.Get(Constants.IMGS);
                result.Url = doc.Get(Constants.URL);
                searchResult.Add(result);
            }
            return searchResult;
        }

        /// <summary>
        /// 按页从索引库中检索关键字
        /// </summary>
        /// <param name="kewWords">关键字</param>
        /// <param name="startRowIndex">第几页</param>
        /// <param name="pagesize">页大小</param>
        /// <param name="totalHits">匹配总数</param>
        /// <returns></returns>
        public List<MetaSource> SearchFromIndexDataByPage(string kewWords, int startRowIndex, int pagesize, out int totalHits)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(IndexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件
            BooleanQuery query=new BooleanQuery();
            PhraseQuery titleQuery = new PhraseQuery();
            PhraseQuery contentQuery = new PhraseQuery();

            //把用户输入的关键字进行分词
            foreach (string word in SplitContent.SplitWords(kewWords))
            {
                titleQuery.Add(new Term(Constants.TITILE, word));
                contentQuery.Add(new Term(Constants.CONTENT, word));
            }
            //query.Add(new Term("content", "C#"));//多个查询条件时 为且的关系
            //指定关键词相隔最大距离
            titleQuery.Slop = 20;
            contentQuery.Slop = 100;

            // 建立标题与内容或的关系
            query.Add(new BooleanClause(titleQuery, Occur.MUST));
            query.Add(new BooleanClause(contentQuery, Occur.MUST));

            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.Create(500, true);

            //根据query查询条件进行查询，查询结果放入collector容器
            searcher.Search(query, null, collector);
            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            int endIndex = 
                startRowIndex * pagesize > collector.TotalHits ? collector.TotalHits : startRowIndex * pagesize;
            ScoreDoc[] docs = collector.TopDocs((startRowIndex - 1) * pagesize, endIndex).ScoreDocs;

            //展示数据实体对象集合
            List<MetaSource> searchResult = new List<MetaSource>();
            for (int i = 0; i < docs.Length; i++)
            {
                //得到查询结果文档的id（Lucene内部分配的id）
                int docId = docs[i].Doc;
                //根据文档id来获得文档对象Document
                Document doc = searcher.Doc(docId);
                MetaSource result = new MetaSource();
                //搜索关键字高亮显示 使用盘古提供高亮插件
                result.Title = SplitContent.HightLight(kewWords, doc.Get(Constants.TITILE));
                result.Content = SplitContent.HightLight(kewWords, doc.Get(Constants.CONTENT));
                result.Time = doc.Get(Constants.TIME);
                result.Imgs = doc.Get(Constants.IMGS);
                result.Url = doc.Get(Constants.URL);
                searchResult.Add(result);
            }
            totalHits = collector.TotalHits;
            return searchResult;
        }
    }
}
