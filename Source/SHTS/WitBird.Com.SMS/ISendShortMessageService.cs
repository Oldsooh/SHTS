namespace WitBird.Com.SMS
{
    /// <summary>
    /// 发送短消息接口。
    /// </summary>
    public interface ISendShortMessageService
    {
        SMSResponse SendShortMessage(ShortMessage shortmessage);
    }
}
