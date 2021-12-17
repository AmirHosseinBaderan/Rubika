# Rubika

How Use It?

Create an instace of IBot

```  
IBot _bot = new Bot("auth");
```

Now you can use bot functions

Get new messages from chat

```
await _bot.CreateBotAsync((msg)=>{
if(msg.Status == ActionStatus.Success)
{}
},"chatToken");
```


