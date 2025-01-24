// Initialize variables
var quizId = 0;
var createDiv = 0;
var editQuiz = 0;
let output2 = " ";

// Wait for DOM to load before executing
document.addEventListener('DOMContentLoaded', function () {
    // Hide the quiz creation form initially
    createDiv = document.getElementById('create-quiz-container');
    createDiv.querySelector('#create-quiz-form').style.display = 'none';

    // Event listener to toggle form visibility
    createDiv.querySelector('#quiz-create-button').addEventListener('click', toggleForm);

    // Handle quiz creation form submission
    createDiv.querySelector("#create-quiz-form").addEventListener('submit', function (event) {
        event.preventDefault();
        const formData = new FormData(this);

        // Send form data to server to create the quiz
        fetch("../postQuizzes", {
            method: "POST",
            body: formData
        }).then(response => {
            if (response.ok) return;
            throw new Error('Fetch delete failed.');
        }).then(() => {
            location.reload(); // Reload page after successful creation
        });
    });
});

// Fetch quiz data when page loads
window.onload = fetchQuizData;

// Function to fetch quizzes and display them
function fetchQuizData() {
    fetch("../getQuizzes")
        .then(response => response.json())
        .then(data => {
            console.log(data);

            let output = '';
            data.forEach(item => {
                const message = item.Name;
                const number = item.Id;
                quizId = item.Id;

                console.log('Item:', item);
                console.log('Name:', message, 'ID:', number, 'SID:', item.Id);

                // Generate HTML output for each quiz
                output += `
                    <div class="container">
                        <h1>${message}</h1>
                        <div class="edit-quiz-container">
                            <form class="quiz-edit-form" id="edit-${number}" enctype="multipart/form-data">
                                <input type="hidden" name="id" value="${number}" />
                                <input type="hidden" name="quizName" value="${message}" />
                                <input required id="new-quiz-name" type="text" name="newQuizName" value="${message}" placeholder="Name" />
                                Image <input required type="file" name="FileName" accept="image/*" />
                                <button type="button" id="${number}" onclick="submitUpdateQuiz(this.id)">Submit</button>
                            </form>
                            <button class="quiz-select-button">Edit Questions</button>
                            <button class="quiz-delete-button" id="${number}" onclick="deleteQuiz(this.id)">Delete</button>
                            <button class="quiz-edit-button">Edit</button>
                        </div>
                    </div>`;

            });

            // Insert output into the DOM
            document.getElementById("test").innerHTML = output;

            // Set up listeners for "Edit Questions" buttons
            document.querySelectorAll(".quiz-select-button").forEach(Select => {
                Select.addEventListener('click', function (event) {
                    event.preventDefault();
                    handleQuizSelection(event, Select);
                });
            });

            // Hide all quiz edit forms initially
            document.querySelectorAll(".quiz-edit-form").forEach(form => form.style.display = 'none');

            // Set up listeners for "Edit" buttons
            document.querySelectorAll('.quiz-edit-button').forEach(button => {
                button.addEventListener('click', toggleEditForm);
            });
        })
        .catch(error => console.error('Error fetching quizzes:', error));
}

// Handle the deletion of a quiz
function deleteQuiz(buttonId) {
    event.preventDefault(); // Prevent default behavior (form submission)

    // Construct the data to send, in this case only the ID
    const formData = new FormData();
    formData.append('id', buttonId); // Append the buttonId to the form data

    // Send the DELETE request with the form data
    fetch('deleteQuestions', {
        method: 'DELETE',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Fetch delete failed.');
            }
            return response.json();
        })
        .then(() => {
            console.log('Quiz deleted successfully.');
            location.reload(); // Reload the page after successful deletion
        })
        .catch(error => {
            console.error('Error:', error);
        });
}


// Handle the deletion of a question
function deleteQuestionData(buttonId) {
    const quizId = buttonId;
    const form = document.getElementById(`edit-form-${quizId}`);
    const formData = new FormData(form);

    fetch("../deleteQuestions", {
        method: 'DELETE',
        body: formData
    })
        .then(response => {
            if (response.ok) return;
            throw new Error('Fetch delete failed.');
        })
        .then(() => location.reload()) // Reload page after deletion
        .catch(error => console.error('Error:', error));
}

// Function to toggle the visibility of the quiz creation form
function toggleForm(e) {
    const form = document.getElementById('create-quiz-form');
    if (e.target.innerHTML == "Create") {
        form.style.display = '';
        e.target.innerHTML = "Cancel";
    } else {
        form.style.display = 'none';
        e.target.innerHTML = "Create";
    }
};

// Function to toggle the visibility of quiz edit forms
function toggleEditForm(e) {
    const container = e.target.parentNode;
    const form = container.querySelector('.quiz-edit-form');
    if (e.target.innerHTML == "Edit") {
        form.style.display = '';
        e.target.innerHTML = "Cancel";
    } else {
        form.style.display = 'none';
        e.target.innerHTML = "Edit";
    }
};

// Function to handle the editing of questions for a quiz
function handleQuizSelection(event, Select) {
    let output = '';
    document.getElementById('back-button').innerHTML = 'Back to Edit Quizzes';
    document.getElementById('back-button').onclick = () => location.reload();

    const mainDiv = document.querySelector("#mainDiv");
    const container = Select.closest('.edit-quiz-container');
    const quizId = container.querySelector('input[name="id"]').value;
    const quizName = container.querySelector('input[name="quizName"]').value;

    output += `
        <div><h2>Edit ${quizName}</h2>
        <button class="question-add-button" id="form-toggle" onclick="toggleNewForm()">Add Question</button>
        </div>
        <form id="new-question-form" style="display: none;">
            <input type="hidden" name="id" value="${quizId}" />
            <input type="hidden" name="quizName" value="${quizName}" />
            <input type="textarea" name="Question" placeholder="Question" required />
            <input type="textarea" name="Answer" placeholder="Answer" required />
            <input type="textarea" name="Decoy1" placeholder="Decoy answer" required />
            <input type="textarea" name="Decoy2" placeholder="Decoy answer (optional)" />
            <input type="textarea" name="Decoy3" placeholder="Decoy answer (optional)" /><br>
            Content type: <input type="radio" id="content-quote" name="ContentType" value="quote">
            <label for="content-quote">Quote</label>
            <input type="radio" id="content-image" name="ContentType" value="image">
            <label for="content-image">Image</label>
            <input type="radio" id="content-audio" name="ContentType" value="audio">
            <label for="content-audio">Audio</label>
            <input type="radio" id="content-video" name="ContentType" value="video">
            <label for="content-video">Video</label><br>
            Content: <input class="file-input" type="file" name="FileName" />
            <input class="quote-input" type="text" name="QuoteText" placeholder="Quote" required />
            <br><button type="button" onclick="submitFormData()">Submit</button>
        </form>
        <div id="target"> target </div>`;

    mainDiv.innerHTML = output;

    // Fetch existing questions for the quiz
    fetchQuizQuestions(quizId, quizName);
}

// Function to fetch questions for the selected quiz
function fetchQuizQuestions(quizId, quizName) {
    const formData = new FormData();
    formData.append('id', quizId);
    formData.append('quizName', quizName);

    const queryString = new URLSearchParams(formData).toString();

    fetch(`getQuestions?id=${quizId}&quizName=${encodeURIComponent(quizName)}`, { method: 'GET'})
        .then(response => response.json())
        .then(data => {
            let output2 = '';
            data.forEach(quest => {
                console.log(quest);
                const questId = quest.QuestId;
                const corr = quest.Corr;
                const dec1 = quest.Dec1;
                const dec2 = quest.Dec2;
                const dec3 = quest.Dec3;
                const mediaTyp = quest.MediaTyp;
                const mediaPrev = quest.MediaPrev;
                const question = quest.Question;

                output2 += `
                    <div class="questions-div">
                        <h2>${question}</h2>
                        <div id="edit-question-${quizId}" class="question-edit-container" questionId="${questId}">
                            ${question}
                            <form class="question-edit-form" id="edit-form-${questId}" style="display: none;">
                                <input type="hidden" name="id" value="${quizId}" />
                                <input type="hidden" name="quizName" value="${quizName}" />
                                <input type="hidden" name="questionId" value="${questId}" />
                                <input type="textarea" name="Question" placeholder="Question text" value="${question}" required />
                                <input type="textarea" name="Answer" placeholder="Answer" value="${corr}" required />
                                <input type="textarea" name="Decoy1" placeholder="Decoy answer" value="${dec1}" required />
                                <input type="textarea" name="Decoy2" placeholder="Decoy answer (optional)" value="${dec2}" />
                                <input type="textarea" name="Decoy3" placeholder="Decoy answer (optional)" value="${dec3}" /><br>
                                Content type: <input type="hidden" name="selectedContent" value="${mediaTyp}" />
                                <input type="radio" class="radio-default" id="content-quote" name="ContentType" value="quote">
                                <label for="content-quote">Quote</label>
                                <input type="radio" id="content-image" name="ContentType" value="image">
                                <label for="content-image">Image</label>
                                <input type="radio" id="content-audio" name="ContentType" value="audio">
                                <label for="content-audio">Audio</label>
                                <input type="radio" id="content-video" name="ContentType" value="video">
                                <label for="content-video">Video</label><br>
                                Content: <p>Current: ${mediaPrev}</p>
                                <input class="file-input" type="file" name="FileName" />
                                <input class="quote-input" type="text" name="QuoteText" placeholder="Quote" />
                                <br><button id="${questId}" onclick="submitUpdateData(this.id)">Submit</button>
                            </form>
                            <button class="question-edit-toggle" id="${questId}" onclick="toggleEditsForm(this.id)">Edit</button>
                            <button class="question-delete" id="${questId}" onclick="deleteQuestionData(this.id)">Delete</button><br>
                        </div>
                    </div>`;
            });

            // Display the questions
            document.querySelector("#target").innerHTML = output2;
        })
        .catch(error => console.error('Error fetching questions:', error));
}

// Toggle visibility of the new question form
function toggleNewForm() {
    const form = document.getElementById('new-question-form');
    form.style.display = form.style.display === 'none' ? 'block' : 'none';
}

// Toggle visibility of question edit forms
function toggleEditsForm(buttonId) {
    const form = document.getElementById(`edit-form-${buttonId}`);
    form.style.display = form.style.display === "none" || form.style.display === "" ? "block" : "none";
}

// Submit new question form data
function submitFormData() {
    const form = document.getElementById("new-question-form");
    const formData = new FormData(form);

    fetch('../postQuestions', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            location.reload(); // Reload page after successful submission
        })
        .catch(error => console.error('Error:', error));
}

// Submit quiz update data
function submitUpdateQuiz(buttonId) {
    event.preventDefault();
    const form = document.getElementById(`edit-${buttonId}`);
    const formData = new FormData(form);

    fetch("../editCategories", {
        method: 'PUT',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Update failed with status: ' + response.status);
            }
            return response.json();
        })
        .then(data => {
            console.log('Update Successful:', data.message); // Log success message or data
            location.reload(); // Reload after update
        })
        .catch(error => console.error('Error:', error));

}

// Submit question update data
function submitUpdateData(buttonId) {
    event.preventDefault();
    const form = document.getElementById(`edit-form-${buttonId}`);
    const formData = new FormData(form);
    const baseUrl = window.location.origin;

    fetch(`../updateQuest`, {
        method: 'PUT',
        body: formData
    })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            location.reload(); // Reload after update
        })
        .catch(error => console.error('Error:', error));
}
