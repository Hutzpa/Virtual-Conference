const express = require('express');
const app = require('express')();
const http = require('http').createServer(app);
const io = require('socket.io')(http);

http.listen(3000, () => console.log("Listening 3000"));

io.on('connection', socket => {
    console.log(`${socket.client.id} has been connected`) //лог подключения

    socket.on('greetingToServ', room => {
        console.log(`Preparing to connect to ${room}`);
        socket.join(room);
        console.log(`User ${socket.client.id} connected to room ${room}`);
    });

    socket.on('chatMessage', Message => {
        console.log(Message);
        socket.broadcast.to(Message.room).emit('chat message', { name: Message.name, msg: Message.msg });
    });

    socket.on('disconnect', () => {
        console.log(`${socket.client.id} disconnected`);
    }); //лог отключения
});