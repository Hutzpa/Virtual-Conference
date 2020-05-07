//var $ = require('jquery');
//var socket = io("http://localhost:3000");

var socket = io("http://localhost:3000");

//Text chat
socket.emit("greetingToServ", document.getElementById('idEvent').value + document.getElementById('idRoom').value);
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
socket.on('chat message', Message => {
    $('#all_mess').append(`<div class='alert'> <b>${Message.name} </b> : ${Message.msg} </div>`);
});

//Video stream

const videoElem = document.getElementById("video");
const logElem = document.getElementById("log");
const startElem = document.getElementById("start");
const stopElem = document.getElementById("stop");

// Options for getDisplayMedia()

var displayMediaOptions = {
    video: {
        cursor: "always"
    },
    audio: true
};

// Set event listeners for the start and stop buttons
startElem.addEventListener("click", function (evt) {
    startCapture();
}, false);

stopElem.addEventListener("click", function (evt) {
    stopCapture();
}, false);

console.log = msg => logElem.innerHTML += `${msg}<br>`;
console.error = msg => logElem.innerHTML += `<span class="error">${msg}</span><br>`;
console.warn = msg => logElem.innerHTML += `<span class="warn">${msg}<span><br>`;
console.info = msg => logElem.innerHTML += `<span class="info">${msg}</span><br>`;

async function startCapture() {
    logElem.innerHTML = "";

    try {
        //videoElem.srcObject = await navigator.mediaDevices.getDisplayMedia(displayMediaOptions);
        var frame = await navigator.mediaDevices.getDisplayMedia(displayMediaOptions);
        console.log(frame);
        dumpOptionsInfo();
    } catch (err) {
        console.error("Error: " + err);
    }
}

function stopCapture(evt) {
    let tracks = videoElem.srcObject.getTracks();

    tracks.forEach(track => track.stop());
    videoElem.srcObject = null;
}

function dumpOptionsInfo() {
    const videoTrack = videoElem.srcObject.getVideoTracks()[0];

    console.info("Track settings:");
    console.info(JSON.stringify(videoTrack.getSettings(), null, 2));
    console.info("Track constraints:");
    console.info(JSON.stringify(videoTrack.getConstraints(), null, 2));
}