// "use strict";

var hostId;
var Chat = {};

Chat.socket = null;

Chat.connect = (function (host) {
    if ('WebSocket' in window) {
        Chat.socket = new WebSocket(host);
    } else if ('MozWebSocket' in window) {
        Chat.socket = new MozWebSocket(host);
    } else {
        console.log('Error: WebSocket is not supported by this browser.');
        return;
    }

    Chat.socket.onopen = function () {
        console.log('Info: WebSocket connection opened.');
    };

    Chat.socket.onclose = function () {
        // document.getElementById('chat').onkeydown = null;
        console.log('Info: WebSocket closed.');
    };

    Chat.socket.onmessage = function (message) {
        try {
            const parsedMessage = JSON.parse(message.data)
            console.log(parsedMessage);
            if (parsedMessage.dataType == 'newQuestion')
                setAnswers(parsedMessage)
            if (!hostId && parsedMessage.dataType == 'resendQuestion') {
                console.log('joining late')
                setAnswers(parsedMessage)
            }
            if (parsedMessage.dataType == 'hostAnswered')
                showCorrect(parsedMessage)
            if (parsedMessage.dataType == 'disconnect')
                clearQuestion(parsedMessage)
        } catch (e) {
            console.log("Failed to set question details: " + e);
            console.log("Message: " + message.data)
        }
    };
});

Chat.initialize = function () {
    if (window.location.protocol == 'http:') {
        Chat.connect('ws://' + window.location.host + '/trivia/websocket/quiz');
    } else {
        Chat.connect('wss://' + window.location.host + '/trivia/websocket/quiz');
    }
};

Chat.initialize();

const question = document.getElementById('question-text')
console.log(question)
const answerContainer = document.getElementById('answer-container')
console.log(answerContainer)

function setAnswers(message) {
    hostId = message.id
    console.log('in set answers')
    answerContainer.innerHTML = ''
    question.innerHTML = message.message
    message.answers.forEach(answer => {
        const button = document.createElement('button')
        button.innerHTML = answer.text
        button.onclick = selectAnswer
        button.dataset.answerId = answer.id
        answerContainer.appendChild(button)
    })
}

function selectAnswer(e) {
    document.querySelector('#answer-container').querySelectorAll('button').forEach(button => {
        button.classList.remove('selected')
    })
    console.log(e.target)
    e.target.classList.add('selected')
    console.log(e.target.dataset.answerId)
    const message = {}
    message.dataType = 'remoteSelection'
    message.message = 'Selected: ' + e.target.innerHTML
    message.selection = e.target.dataset.answerId
    Chat.socket.send(JSON.stringify(message))
}

function showCorrect(message) {
    document.querySelector('#answer-container').querySelectorAll('button').forEach(button => {
        button.disabled = true
        if (button.dataset.answerId === message.selection) {
            console.log('was correct: ', button)
            button.classList.add('correctButton')
        } else if (button.classList.contains('selected')) {
            console.log('was selected: ', button)
            button.classList.add('incorrectButton')
        }
    })
}

function clearQuestion(message) {
    if (message.id == hostId) {
        answerContainer.innerHTML = ''
        question.innerHTML = 'Waiting for quiz host...'
    }
}
