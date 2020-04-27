const express = require('express');
const app = require('express')();
const http = require('http').createServer(app);
const io = require('socket.io')(http);

//app.use("/static", express.static("./static/"));
//app.get('/', (req, res) => res.sendFile(__dirname + '/static/client.html'));
http.listen(3000, () => console.log("Listening 3000"));

console.log("dude pls");

io.on('connection', socket => {
    console.log(`${socket.client.id} has been connected`) //лог подключения

    socket.on('chat mes', Message => {
        console.log(Message);
        socket.broadcast.emit('chat message', { name: Message.name, msg: Message.msg });
    });

    socket.on('disconnect', () => {
        console.log(`${socket.client.id} disconnected`);
    }); //лог отключения
});