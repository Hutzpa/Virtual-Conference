
var socket = io("http://localhost:3000");

//Text chat
socket.emit("greetingToServ", document.getElementById('idEvent').value + document.getElementById('idRoom').value);

socket.on('chat message', Message => {
    $('#all_mess').append(`<div class='alert'> <b>${Message.name} </b> : ${Message.msg} </div>`);
});


function CreateMes(idEvent, idRoom, userName, message) {
    return { room: idEvent + idRoom, name: userName, msg: message }
}

$('sent').on('click', e => {
    console.log("jgerijgijerijgjierigjoerjigiojerojigjergjioerijoger");

});
$('form').submit(e => {

    console.log("Сюда дошло");
    e.preventDefault();
    console.log(CreateMes(document.getElementById('idEvent').value,
        document.getElementById('idRoom').value, document.getElementById('name').value, document.getElementById('message').value));
    socket.emit('chatMessage', CreateMes(document.getElementById('idEvent').value,
        document.getElementById('idRoom').value, document.getElementById('name').value, document.getElementById('message').value));
  
    $('#all_mess').append(`<div class='alert'> <b> ${$('#name').val()} </b> : ${$('#message').val()} </div>`)
    $('#message').val('');
    return false;
});


