// // Set up new question form
const createDiv = document.getElementById('create-quiz-container')
createDiv.querySelector('#create-quiz-form').style.display = 'none';

// // Add content visibility toggling functions to edit buttons and radio buttons
createDiv.querySelector('#quiz-create-button').addEventListener('click', toggleForm);

// Set up forms for editing each existing question
document.querySelectorAll('.quiz-edit-container').forEach(container => {
    const form = container.querySelector('.quiz-edit-form')
    console.log(form)
    form.style.display = 'none'

    container.querySelector('.quiz-edit-button').addEventListener('click', toggleEditForm)

    const quizId = container.querySelector('input[name="id"]').value
    const quizName = container.querySelector('input[name="quizName"').value
    container.querySelector('.quiz-questions-button').addEventListener('click', () => {
        window.location='/trivia/editQuestions?id=' + quizId + '&quizName=' + quizName
    })
    
    // Set up delete button
    container.querySelector('.quiz-delete-button').addEventListener('click', function() {
        const url = '/trivia/editQuizzes?id=' + quizId
    
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
            location.reload()
        })
        .catch(error => {
            console.error('Error:', error);
        });
    });
});


function toggleForm(e) {
    if (e.target.innerHTML == "Create") {
        document.getElementById('create-quiz-form').style.display = '';
        e.target.innerHTML = "Cancel";
    } else {
        document.getElementById('create-quiz-form').style.display = 'none';
        e.target.innerHTML = "Create";
    }
}


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
}
