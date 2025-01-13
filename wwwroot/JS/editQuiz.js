var quizId = 0;
var createDiv = 0;
var editQuiz = 0;
let output2 = " ";

// // Set up new question form
document.addEventListener('DOMContentLoaded', function () {
    createDiv = document.getElementById('create-quiz-container');
    createDiv.querySelector('#create-quiz-form').style.display = 'none';

    createDiv.querySelector('#quiz-create-button').addEventListener('click', toggleForm);


    createDiv.querySelector("#create-quiz-form").addEventListener('submit', function (event) {
        event.preventDefault();


        const formData = new FormData(this);

        fetch("../editQuizzes", {
            method: "POST",
            body: formData
        }).then(response => {
            if (response.ok) {
                return
            }
            throw new Error('Fetch delete failed.');
        }).then(() => {
            location.reload();
        })
    })



})

window.onload = fetchQuizData;

function fetchQuizData() {
    fetch("../editQuizzes")
        .then(response => {
            console.log(response)
            return response.json()
        })
        .then(data => {
            console.log(data);
            let output = '';

            data.forEach(item => {
                const message = item.name;
                const number = item.sid;

                quizId = item.sid;

                console.log('Message: ', message, ', Number: ', number);



                output += "<div class=\"container\"><h1>" +
                    message + "</h1>" +
                    "<div class=\"edit-quiz-container\">" +
                    "<form class=\"quiz-edit-form\" id=\"edit-" + number + "\" enctype=\"multipart/form-data\">" +
                    "<input type=\"hidden\" name=\"id\" value=\"" + number + "\"/>" +
                    "<input type=\"hidden\" name=\"quizName\" value=\"" + message + "\"/>" +
                    "<input required id=\"new-quiz-name\" type=\"text\" name=\"QuizName\" " +
                    "value=\"" + message + "\" placeholder=\"Name\" />" +
                    "Image <input required type=\"file\" name=\"FileName\" accept=\"image/*\" />" +
                    "<button type=\"button\" id=\"" + number + "\" onclick=\"submitUpdateQuiz(this.id)\">Submit</button>" + // Updated button
                    "</form>" +
                    "<button class=\"quiz-select-button\">Edit Questions</button>" +
                    "<button class=\"quiz-delete-button\" id=\"" + number + "\" onclick=\"deleteQuiz(this.id)\">Delete</button>" +
                    "<button class=\"quiz-edit-button\">Edit</button>" +
                    "</div></div>\n";

            })

            document.getElementById("test").innerHTML = output;

            document.querySelectorAll(".quiz-select-button").forEach(Select => {
                Select.addEventListener('click', function (event) {
                    event.preventDefault();

                    let output = '';

                    document.getElementById('back-button').innerHTML = 'Back to Edit Quizzes'
                    document.getElementById('back-button').onclick = function() {
                        location.reload()
                    }

                    const mainDiv = document.querySelector("#mainDiv");


                    const container = Select.closest('.edit-quiz-container');
                    const formData = new FormData();

                    const quizId = container.querySelector('input[name="id"]').value;
                    const quizName = container.querySelector('input[name="quizName"]').value;

                    console.log(quizId);
                    console.log(quizName);

                    output += "<div><h2>Edit " + quizName + "</h2>" +
                        "<button class=\"question-add-button\" id=\"form-toggle\" onclick=\"toggleNewForm()\">Add Question</button>" +
                        "</div>" + "<form id=\"new-question-form\" style=\"display: none;\">" +
                        "<input type=\"hidden\" name=\"id\" value=\"" + quizId + "\" />" +
                        "<input type=\"hidden\" name=\"quizName\" value=\"" + quizName + "\" />" +
                        "<input type=\"textarea\" name=\"Question\" placeholder=\"Question\" required />" +
                        "<input type=\"textarea\" name=\"Answer\" placeholder=\"Answer\" required />" +
                        "<input type=\"textarea\" name=\"Decoy1\" placeholder=\"Decoy answer\" required />" +
                        "<input type=\"textarea\" name=\"Decoy2\" placeholder=\"Decoy answer (optional)\" />" +
                        "<input type=\"textarea\" name=\"Decoy3\" placeholder=\"Decoy answer (optional)\" /><br>" +
                        "Content type: <input type=\"radio\" id=\"content-quote\" name=\"ContentType\" value=\"quote\">" +
                        "<label for=\"content-quote\">Quote</label>" +
                        "<input type=\"radio\" id=\"content-image\" name=\"ContentType\" value=\"image\">" +
                        "<label for=\"content-image\">Image</label>" +
                        "<input type=\"radio\" id=\"content-audio\" name=\"ContentType\" value=\"audio\">" +
                        "<label for=\"content-audio\">Audio</label>" +
                        "<input type=\"radio\" id=\"content-video\" name=\"ContentType\" value=\"video\">" +
                        "<label for=\"content-video\">Video</label><br>" +
                        "Content: " +
                        "<input class=\"file-input\" type=\"file\" name=\"FileName\" />" +
                        "<input class=\"quote-input\" type=\"text\" name=\"QuoteText\" placeholder=\"Quote\" required />" +
                        "<br><button type=\"button\" onclick=\"submitFormData()\">Submit</button>" +
                        "</form><div id=\"target\"> target </div>";


                    mainDiv.innerHTML = output;

                    formData.append('id', quizId);
                    formData.append('quizName', quizName);

                    const queryString = new URLSearchParams(formData).toString();

                    fetch(`../editQuestions?${queryString}`, {
                        method: 'GET'
                    }).then(response => {
                        console.log(response);
                        if (response.ok) {
                            return response.json();
                        }
                        throw new Error('fetch failed');
                    })
                        .then(data => {
                            console.log(data);

                            if (data === null) { // Check if data is null
                                console.log('No data received, stopping execution.');
                                return; // Stop further execution if data is null
                            }

                            data.forEach(quest => {

                                const questId = quest.sid;

                                const corr = quest.corrAns;
                                const dec1 = quest.decAns1;
                                const dec2 = quest.decAns2;
                                const dec3 = quest.decAns3;

                                const mediaTyp = quest.media_type;
                                const mediaPrev = quest.media_preview;

                                const question = quest.question;

                                output2 += "<div class=\"questions-div\"><h2>" + question + "</h2>" +
                                    "<div id=\"edit-question-" + quizId + "\" class=\"question-edit-container\" " +
                                    "questionId=\"" + questId + "\">" + question +
                                    "<form class=\"question-edit-form\" id=\"edit-form-" + questId
                                    + "\" style=\"display: none\;\">" +
                                    "<input type=\"hidden\" name=\"id\" value=\"" + quizId + "\" />" +
                                    "<input type=\"hidden\" name=\"quizName\" value=\"" + quizName + "\" />" +
                                    "<input type=\"hidden\" name=\"questionId\" value=\"" + questId + "\" />" +
                                    "<input type=\"textarea\" name=\"Question\" placeholder=\"Question text\" " +
                                    "value=\"" + question + "\" required />" +
                                    "<input type=\"textarea\" name=\"Answer\" placeholder=\"Answer\" " +
                                    "value=\"" + corr + "\" required />" +
                                    "<input type=\"textarea\" name=\"Decoy1\" placeholder=\"Decoy answer\" " +
                                    "value=\"" + dec1 + "\" required />" +
                                    "<input type=\"textarea\" name=\"Decoy2\" placeholder=\"Decoy answer (optional)\" " +
                                    "value=\"" + dec2 + "\" />" +
                                    "<input type=\"textarea\" name=\"Decoy3\" placeholder=\"Decoy answer (optional)\" " +
                                    "value=\"" + dec3 + "\" /><br>" +
                                    "Content type: " +
                                    "<input type=\"hidden\" name=\"selectedContent\" value=\"" + mediaTyp + "\" />" +
                                    "<input type=\"radio\" class=\"radio-default\" id=\"content-quote\" name=\"ContentType\" value=\"quote\">"
                                    +
                                    "<label for=\"content-quote\">Quote</label>" +
                                    "<input type=\"radio\" id=\"content-image\" name=\"ContentType\" value=\"image\">" +
                                    "<label for=\"content-image\">Image</label>" +
                                    "<input type=\"radio\" id=\"content-audio\" name=\"ContentType\" value=\"audio\">" +
                                    "<label for=\"content-audio\">Audio</label>" +
                                    "<input type=\"radio\" id=\"content-video\" name=\"ContentType\" value=\"video\">" +
                                    "<label for=\"content-video\">Video</label><br>" +
                                    "Content: " +
                                    "<p>Current: " + mediaPrev + "</p>" +
                                    "<input class=\"file-input\" type=\"file\" name=\"FileName\" />" +
                                    "<input class=\"quote-input\" type=\"text\" name=\"QuoteText\" placeholder=\"Quote\" />" +
                                    "<br><button id=\"" + questId + "\" onclick=\"submitUpdateData(this.id)\">Submit</button>" +
                                    "</form>" +
                                    "<button class=\"question-edit-toggle\" id=\"" + questId + "\" onclick=\"toggleEditsForm(this.id)\">Edit</button>" +
                                    "<button class=\"question-delete\" id=\"" + questId + "\" onclick=\"deleteQuestionData(this.id)\">Delete</button><br>" +
                                    "</div>";


                            })

                            document.querySelector("#target").innerHTML = output2;




                        })
                        .catch(error => {
                            console.error('Error: ', error);
                            alert('error fetching questions');
                        })
                })
            })

            document.querySelectorAll(".quiz-edit-form").forEach(form => {
                form.style.display = 'none';
            })

            document.querySelectorAll('.quiz-edit-button').forEach(button => {
                button.addEventListener('click', toggleEditForm);
            })






            var deleteButton = document.querySelectorAll('.quiz-delete-button');

        })
        .catch(error => console.error('error Fetching Json: ', error))
}

function deleteQuiz(buttonId) {
    const url = '/trivia/editQuizzes?id=' + buttonId;

    fetch(url, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                return
            }
            throw new Error('Fetch delete failed.');
        })
        .then(() => {
            location.reload();
        })
        .catch(error => {
            console.error('Error:', error);
        });
}


function deleteQuestionData(buttonId) {
    const quizId = buttonId;

    console.log(quizId);



    // Get the form associated with this quizId
    const form = document.getElementById(`edit-form-${quizId}`);

    // Create a FormData object from the form
    const formData = new FormData(form);

    // Send the DELETE request
    fetch("../editQuestions", {
        method: 'DELETE',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                return
            }
            throw new Error('Fetch delete failed.');
        })
        .then(() => {
            location.reload(); // Reload the page on successful deletion
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

// Set up delete button


function toggleForm(e) {
    if (e.target.innerHTML == "Create") {
        document.getElementById('create-quiz-form').style.display = '';
        e.target.innerHTML = "Cancel";
    } else {
        document.getElementById('create-quiz-form').style.display = 'none';
        e.target.innerHTML = "Create";
    }
};

function toggleEditForm(e) {
    const container = e.target.parentNode;
    const form = container.querySelector('.quiz-edit-form')
    if (e.target.innerHTML == "Edit") {
        form.style.display = '';
        e.target.innerHTML = "Cancel";
    } else {
        form.style.display = 'none';
        e.target.innerHTML = "Edit";
    }
};

function toggleNewForm() {
    const form = document.getElementById('new-question-form');
    if (form.style.display === 'none') {
        form.style.display = 'block';
    } else {
        form.style.display = 'none';
    }
};

function toggleEditsForm(buttonId) {
    // Extract quizId from the buttonId
    const quizId = buttonId;

    console.log(quizId);
    // Get the form associated with this quizId
    const form = document.getElementById(`edit-form-${quizId}`);

    // Toggle the display style
    if (form.style.display === "none" || form.style.display === "") {
        form.style.display = "block"; // Show the form
    } else {
        form.style.display = "none"; // Hide the form
    }
}


function submitFormData() {
    const form = document.getElementById("new-question-form");
    const formData = new FormData(form);

    fetch('../editQuestions', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return;
        })
        .then(data => {
            location.reload();
        })
        .catch((error) => {
            console.error('Error:', error);
            // Optionally handle error (e.g., show an error message)
        });
}





function submitUpdateQuiz(buttonId) {
    event.preventDefault()
    
    const form = document.getElementById(`edit-${buttonId}`); // Get the form associated with the quiz ID

    // Create a FormData object from the form
    const formData = new FormData(form);

    // Send the POST request to ../editQuizzes
    fetch("../editQuizzes", {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Update failed with status: ' + response.status);
            }
            return response.json(); // Assuming you want to handle a JSON response
        })
        .then(data => {
            console.log('Update successful:', data);
            location.reload(); // Reload the page to see changes
        })
        .catch(error => {
            console.error('Error:', error);
            // Optionally handle error (e.g., show an error message)
        });
}


function submitUpdateData(buttonId) {
    event.preventDefault();
    // Use the buttonId to get the form associated with the button
    const form = document.getElementById(`edit-form-${buttonId}`);

    // Create a FormData object from the form
    const formData = new FormData(form);

    for (let [key, value] of formData.entries()) {
        console.log(`${key}: ${value}`);
    }

    fetch('../editQuestions', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return  // If you expect a JSON response
        })
        .then(data => {
            // Optionally handle the response data
            location.reload(); // Reload the page on successful update
        })
        .catch((error) => {
            console.error('Error:', error);
            // Optionally handle error (e.g., show an error message)
        });
}



