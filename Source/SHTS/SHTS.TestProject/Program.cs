using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Witbird.SHTS.BLL.Service;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace SHTS.TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Print("begin...");
            try
            {
                //TestRepository testRepository = new TestRepository();
                //using (TransactionScope scope = new TransactionScope())
                //{
                //    Print("Insert first Test entity...");
                //    var test1 = NewTestEntity(1);
                //    testRepository.AddEntity(test1);

                //    Print("Insert second Test entity...");
                //    var test2 = NewTestEntity(2);
                //    testRepository.AddEntity(test2);

                //    Print("Insert third Test entity...");
                //    Test test3 = null;
                //    testRepository.AddEntity(test3);

                //    scope.Complete();
                //}

                var service = new DemandQuoteService();
                var quote = NewDemandQuotesEntity();
                service.NewQuoteRecord(quote);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch(Exception ex)
            {
                Print("Exception caught...");
                Print(ex.ToString());
            }

            Pause();
        }

        static Test NewTestEntity(int seed)
        {
            return new Test() 
            {
                BrandID = seed,
                BrandLogo = "Brand logo " + seed,
                BrandName = "Brand name " + seed,
                BrandPrefix = "Prefix " + seed,
                DataChange_CreateTime = DateTime.Now,
                DataChange_Lasttime = DateTime.Now,
                DataStatus = seed,
                PKG_PSID = seed,
                ProviderCity = seed,
                ProviderDesc = "Description " + seed,
                ProviderID = seed,
                ProviderLogo = "Provider logo " + seed,
                ProviderName = "Provider Name " + seed,
                ProviderPrefix = "Prefix " + seed
            };
        }

        static DemandQuotes NewDemandQuotesEntity()
        {
            var quotes = new DemandQuotes() 
            {
                AcceptStatus = DemandQuoteStatus.Wait.ToString(),
                ContactName = "Richard",
                ContactPhoneNumber = "13888888888",
                DemandId = 1,
                HandleStatus = false,
                InsertedTimestamp = DateTime.Now,
                IsActive = true,
                LastUpdatedTimestamp = DateTime.Now,
                QuotePrice = 1000,
                WeChatUserId = 1
            };

            var quoteHistory = new DemandQuoteHistory() 
            {
                Comments = "Quote comments",
                HasRead = false,
                InsertedTimestamp = DateTime.Now,
                IsActive = true,
                WeChatUserId = 1
            };

            quotes.QuoteHistories.Add(quoteHistory);

            return quotes;
        }

        static void Print(string value)
        {
            Console.WriteLine(value);
        }

        static void Pause()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
