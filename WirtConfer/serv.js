const express = require('express');
const app = express();
const http = require('http').createServer(app);
const io = require('socket.io')(http);

const PORT = 3000;

let broadcaster;

http.listen(PORT, () => console.log("Listening 3000"));

io.sockets.on("error", e => console.log(e));

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

    socket.on('stream', pack => {
        socket.broadcast.to(pack.room).emit('stream', pack.image);
    });

    socket.on("broadcaster", () => {
        broadcaster = socket.id;
        socket.broadcast.emit("broadcaster");
    });
    socket.on("watcher", () => {
        socket.to(broadcaster).emit("watcher", socket.id);
    });
    socket.on("offer", (id, message) => {
        socket.to(id).emit("offer", socket.id, message);
    });

    socket.on("answer", (id, message) => {
        socket.to(id).emit("answer", socket.id, message);
    });
    socket.on("candidate", (id, message) => {
        socket.to(id).emit("candidate", socket.id, message);
    });
    socket.on("disconnect", () => {
        socket.to(broadcaster).emit("disconnectPeer", socket.id);
    });
    socket.on('disconnect', () => {
        console.log(`${socket.client.id} disconnected`);
    }); 
});