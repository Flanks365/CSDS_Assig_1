//// "use strict";

const questions = [];
const currQuestion = 0;
const answers = []
const answerCounts = {}
const quizData = {}
var showCounts = true;
var Chat = {};
Chat.socket = null;

console.log('test')

if (document.readyState !== 'loading') {
    //Chat.initialize();
} else {
    //document.addEventListener("DOMContentLoaded", function () {
    //    //Chat.initialize();
    //}, false);
    //document.addEventListener("DOMContentLoaded", () => { initializePage() });
}

window.addEventListener('load', () => {
    initializePage();
    if (isAutoplay() == 'true') {
        setAutoplay();
    }
})


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
        Chat.socket.send(JSON.stringify(quizData))
        // document.getElementById('chat').onkeydown = function (event) {
        //     if (event.keyCode == 13) {
        //         Chat.sendMessage();
        //     }
        // };
    };

    Chat.socket.onclose = function () {
        // document.getElementById('chat').onkeydown = null;
        console.log('Info: WebSocket closed.');
    };

    Chat.socket.onmessage = function (message) {
        try {
            const parsedMessage = JSON.parse(message.data)
            console.log(parsedMessage);
            if (parsedMessage.dataType == 'remoteSelection')
                setAnswer(parsedMessage)
            if (parsedMessage.dataType == 'join')
                resendQuestion()
            if (parsedMessage.dataType == 'disconnect')
                removePlayerAnswer(parsedMessage)
        } catch (e) {
            console.log("Failed to parse message details: " + e);
            console.log("Message: " + message.data)
        }
    };
});

Chat.initialize = function () {
    // Remove elements with "noscript" class - <noscript> is not allowed in XHTML
    var noscripts = document.getElementsByClassName("noscript");
    for (var i = 0; i < noscripts.length; i++) {
        noscripts[i].parentNode.removeChild(noscripts[i]);
    }

    const questionText = document.querySelector('.question').querySelector('h3').innerHTML
    console.log(questionText)

    // answers = []
    // answerCounts = {}

    document.querySelector('.answers').querySelectorAll('.button-container').forEach(buttonContainer => {
        const button = buttonContainer.querySelector('button')
        answers.push({ id: button.dataset.answerId, text: button.innerHTML })
        answerCounts[button.dataset.answerId] = new Set()
    })
    console.log(answers)

    //const quizData = {}
    quizData.dataType = "newQuestion"
    quizData.message = questionText
    quizData.answers = answers

    document.getElementById('counter-toggle').addEventListener('click', toggleAnswerCounts)
    // document.querySelector('.answers').querySelectorAll('.button-container').forEach(buttonContainer => {
    //     buttonContainer.querySelector('span').style.display = 'none'
    // })

    // var showCounts = true;
    const urlParams = new URLSearchParams(window.location.search);
    // const myParam = urlParams.get('autoplay');
    showCounts = urlParams.get('showCounts') == 'true' || urlParams.get('showCounts') == null;
    console.log(urlParams.get('showCounts'))
    console.log(showCounts)
    setAnswerCounts(showCounts)


    if (window.location.protocol == 'http:') {
        Chat.connect('ws://' + window.location.host + '/trivia/websocket/quiz');
    } else {
        Chat.connect('wss://' + window.location.host + '/trivia/websocket/quiz');
    }
};


function initializePage() {
    console.log('init test')
    getQuestions();
    initializePageContent();
}


function selectAnswer(element, categoryName, answerId, questionId, autoplay, currentIndex) {
    console.log(element)
    console.log('Fetching answer for questionId:', questionId, 'and answerId:', answerId)
    fetch('getCorrectAnswer?questionId=' + questionId + '&answerId=' + answerId)
        .then(response => response.text())
        .then(result => {
            const buttons = element.parentNode.parentNode.childNodes
            console.log(buttons)
            if (result === 'correct') {
                const message = { dataType: 'hostAnswered', message: 'Correct answer: ' + element.innerHTML, selection: answerId }
                console.log(message)
                Chat.socket.send(JSON.stringify(message))
                toggleButtonsDisabled(buttons)
                element.classList.add('correctButton')
                setTimeout(() => {
                    toggleButtonsDisabled(buttons)
                    element.classList.remove('correctButton')
                    window.location.href = "Quizpage?category_name=" + categoryName +
                        "&autoplay=" + autoplay + "&currentQuestionIndex=" + (currentIndex + 1) +
                        "&showCounts=" + showCounts
                }, 1000)

            } else {
                toggleButtonsDisabled(buttons)
                element.classList.add('incorrectButton')
                setTimeout(() => {
                    toggleButtonsDisabled(buttons)
                    element.classList.remove('incorrectButton')
                }, 500)
            }
        })
        .catch(error => console.error('Error checking the answer:', error))
}


function toggleButtonsDisabled(buttons) {
    buttons.forEach(button => {
        button.querySelector('button').disabled = !button.querySelector('button').disabled
    })
}


function setAutoplay() {
    const correctAnswerID = document.getElementById('autoplayCorrectAnswer').value;
    const questionInfo = document.getElementById('questionInfo');
    let corrButton = document.getElementById(correctAnswerID);
    let secondsRemaining = 10
    questionInfo.innerHTML = "Time left: " + secondsRemaining
    const myInterval = setInterval(() => {
        secondsRemaining--
        questionInfo.innerHTML = "Time left: " + secondsRemaining
        if (!secondsRemaining) {
            const message = { dataType: 'hostAnswered', message: 'Correct answer: ' + corrButton.innerHTML, selection: correctAnswerID }
            console.log(message)
            Chat.socket.send(JSON.stringify(message))
            startAnimation()
        }
    }, 1000)
    function startAnimation() {
        corrButton.classList.add('animation')
        clearInterval(myInterval)
        setTimeout(() => {
            corrButton.classList.remove('animation')
            corrButton.disabled = false
            corrButton.classList.add('correctButton')
            corrButton.click()
        }, 3500)
    }
}


function isAutoplay() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('autoplay') === 'true';
}


function initializePageContent() {
    const urlParams = new URLSearchParams(window.location.search);
    const categoryName = urlParams.get('category_name');
    document.getElementById("quiz-title").innerHTML = `Test your knowledge of the ${categoryName} landscape!`;
    //console.log(questions[currQuestion]);
    //document.getElementById("question-text") = questions[currQuestion].text;
}


function setAnswerCounts(show) {
    const button = document.getElementById('counter-toggle')
    if (show) {
        document.querySelector('.answers').querySelectorAll('.button-container').forEach(buttonContainer => {
            buttonContainer.querySelector('span').style.display = ''
        })
        countAnswers()
        button.innerHTML = "Hide answer counts";
        showCounts = true
    } else {
        document.querySelector('.answers').querySelectorAll('.button-container').forEach(buttonContainer => {
            buttonContainer.querySelector('span').style.display = 'none'
        })
        button.innerHTML = "Show answer counts";
        showCounts = false
        console.log('set false')
    }
}


function toggleAnswerCounts(e) {
    setAnswerCounts(e.target.innerHTML == "Show answer counts")  
}


function setAnswer(message) {
    for (let answer in answerCounts) {
        answerCounts[answer].delete(message.id)
    }
    answerCounts[message.selection].add(message.id)
    if (showCounts) {
        countAnswers()
    }
}


function removePlayerAnswer(message) {
    console.log('in remove')
    for (let answer in answerCounts) {
        answerCounts[answer].delete(message.id)
    }
    // answerCounts[message.selection].add(message.id)
    if (showCounts) {
        countAnswers()
    }
}


function countAnswers() {
    document.querySelector('.answers').querySelectorAll('.button-container').forEach(buttonContainer => {
        const ansId = buttonContainer.querySelector('button').dataset.answerId
        const counter = buttonContainer.querySelector('span')
        // answers.push({id: button.dataset.answerId, text: button.innerHTML})
        counter.innerHTML = answerCounts[ansId].size
    })
}


function resendQuestion() {
    quizData.dataType = 'resendQuestion'
    Chat.socket.send(JSON.stringify(quizData))
}


function getQuestions() {
    const urlParams = new URLSearchParams(window.location.search);
    // const myParam = urlParams.get('autoplay');
    const categoryId = urlParams.get('category_id');
    fetch('questionsFromDb?category_id=' + categoryId)
        .then(response => response.text())
        .then(result => {
            const questionList = JSON.parse(result);
            questionList.forEach(question => {
                questions.push(question);
            });
            console.log('getQuestionsDone');
        })
        .then(setQuestion)
}


function setQuestion() {
    const question = questions[currQuestion]
    document.getElementById("question-text").innerHTML = question.text;
    if (question.mediaType.includes('image')) {
        document.getElementById("quoteOrBlob").appendChild( createImg(question.mediaType, question.mediaContent) );
    }
    //document.getElementById("quoteOrBlob") = question.mediaContent;
    getAnswers(question.id);
}


function createImg(imgType, imgString) {
    const img = document.createElement('img');
    img.src = `data:${imgType};base64,${imgString}`;
    return img;
}


function getAnswers(questionId) {
    fetch('answersFromDb?question_id=' + questionId)
        .then(response => response.text())
        .then(result => {
            const answerList = JSON.parse(result);
            console.log(answerList);
            //questionList.forEach(question => {
            //    questions.push(question);
            //});
            console.log('getAnswersDone');
            return answerList;
        })
        .then(answers => {
            answers.forEach(answer => {
                //console.log(answer);
                const button = document.createElement('button');
                button.innerHTML = answer.answerText;
                const div = document.createElement('div');
                div.classList.add('button-container');
                div.appendChild(button);
                document.querySelector('.answers').appendChild(div);
            })
        })
}