var $ = require('jquery');
var socket = io("http://localhost:3000");
console.log("here");

$('form').submit(e => {
    e.preventDefault();
    socket.emit('chat mes', { name: $('#name').val(), msg: $('#message').val() }); //Отправка сообщений на сервер
    $('#all_mess').append(`<div class='alert'> <b> ${$('#name').val()} </b> : ${$('#message').val()} </div>`)
    $('#message').val('');
    return false;
});

socket.on('chat message', Message => {
    $('#all_mess').append(`<div class='alert'> <b>${Message.name} </b> : ${Message.msg} </div>`);
});
