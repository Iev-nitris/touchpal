# Introduction #

The NetworkSendAction send a string of text across to the currently connected game.

# Syntax #
```
NS:{Data}
```

{Data} is replaced with the relevant text which you want sent across the network.

# Examples #
```
NS:C,9,3001,1.0
```
When this action is fired it will send "C,9,3001,1.0" to the current game connection.