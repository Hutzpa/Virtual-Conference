
var socket = io("http://localhost:3000");

//Text chat
socket.emit("greetingToServ", document.getElementById('idEvent').value + document.getElementById('idRoom').value);

socket.on('chat message', Message => {
    $('#all_mess').append(`<div class='alert'> <b>${Message.name} </b> : ${Message.msg} </div>`);
});


function CreateMes(idEvent, idRoom, userName, message) {
    return { room: idEvent + idRoom, name: userName, msg: message }
}
$('form').submit(e => {
    e.preventDefault();
    console.log(CreateMes(document.getElementById('idEvent').value,
        document.getElementById('idRoom').value, document.getElementById('name').value, document.getElementById('message').value));
    socket.emit('chatMessage', CreateMes(document.getElementById('idEvent').value,
        document.getElementById('idRoom').value, document.getElementById('name').value, document.getElementById('message').value));
  
    $('#all_mess').append(`<div class='alert'> <b> ${$('#name').val()} </b> : ${$('#message').val()} </div>`)
    $('#message').val('');
    return false;
});

//Video taking

let peerConnection;
const config = {
    iceServers: [
        {
            urls: ["stun:stun.l.google.com:19302"]
        }
    ]
};


//const socket = io.connect(window.location.origin);
const video = document.querySelector("video");

socket.on("offer", (id, description) => {
    peerConnection = new RTCPeerConnection(config);
    peerConnection
        .setRemoteDescription(description)
        .then(() => peerConnection.createAnswer())
        .then(sdp => peerConnection.setLocalDescription(sdp))
        .then(() => {
            socket.emit("answer", id, peerConnection.localDescription);
        });
    peerConnection.ontrack = event => {
        video.srcObject = event.streams[0];
    };
    peerConnection.onicecandidate = event => {
        if (event.candidate) {
            socket.emit("candidate", id, event.candidate);
        }
    };
});

socket.on("candidate", (id, candidate) => {
    peerConnection
        .addIceCandidate(new RTCIceCandidate(candidate))
        .catch(e => console.error(e));
});

socket.on("connect", () => {
    socket.emit("watcher");
});

socket.on("broadcaster", () => {
    socket.emit("watcher");
});

socket.on("disconnectPeer", () => {
    peerConnection.close();
});

window.onunload = window.onbeforeunload = () => {
    socket.close();
};

