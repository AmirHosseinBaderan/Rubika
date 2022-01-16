# Rubika

How Use It?

Create an instace of IBot

```  Cs
IBot _bot = new Bot("auth");
```

Now you can use bot functions

Get new messages from chat

``` Cs
await _bot.CreateBotAsync(async(msg)=>{
if(msg.Status == ActionStatus.Success)
{
   await _bot.SendMessage("message",replyId,"chatToken");
   await _bot.SnedMessage("message",null,null);
}
},"chatToken");
```



