// Set up new question form
document.getElementById('content-quote').checked = true;
document.getElementById('new-question-form').style.display = 'none';

// Hide file inputs by default
document.querySelectorAll('.file-input').forEach(input => input.style.display = 'none');
document.querySelectorAll('.quote-input').forEach(input => input.style.display = '');

// Add content visibility toggling functions to edit buttons and radio buttons
const radioButtons = document.querySelectorAll('input[name="ContentType"]');
for (const radioButton of radioButtons) {
    radioButton.addEventListener('change', showContentInput);
}
document.getElementById('form-toggle').addEventListener('click', toggleForm);

// Set up forms for editing each existing question
document.querySelectorAll('.question-edit-container').forEach(container => {
    const form = container.querySelector('.question-edit-form')
    form.style.display = 'none'
    const mediaType = container.querySelector('input[name="selectedContent"').value
    container.querySelector('#content-' + mediaType).checked = true;

    const fileInput = form.querySelector('.file-input');
    const quoteInput = form.querySelector('.quote-input');

    if (mediaType == "quote") {
        fileInput.style.display = 'none';
        quoteInput.style.display = '';

        fileInput.required = false;
        quoteInput.required = true;
    } else {
        fileInput.style.display = '';
        fileInput.accept = mediaType + '/*';
        quoteInput.style.display = 'none';

        fileInput.required = true;
        quoteInput.required = false;
    }

    // Set up delete button
    const quizId = container.querySelector('input[name="id"]').value
    const quizName = container.querySelector('input[name="quizName"]').value
    const questionId = container.querySelector('input[name="questionId"]').value
    document.getElementById('question-edit-toggle-' + questionId).addEventListener('click', toggleEditForm)
    container.querySelector('.question-delete').addEventListener('click', function() {
        const url = '/trivia/editQuestions?id=' + quizId + '&quizName=' + quizName + '&questionId=' + questionId
    
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

// async function sendDeleteReq(e, quizId, quizName, questionId) {
//     e.preventDefault();
//     console.log(quizId)
//     console.log(quizName)
//     console.log(questionId)

//     const formData = new FormData();
//     formData.append('id', quizId);
//     formData.append('quizName', quizName);
//     formData.append('questionId', questionId);

//     const url = '/trivia/editQuestions?id=' + quizId + '&quizName=' + quizName + '&questionId=' + questionId

//     const response = await fetch('/trivia/editQuestions', {
//         method: 'DELETE',
//         // body: formData
//     })

//     console.log(response)
// }

function showContentInput(e) {
    const form = e.target.parentNode;
    const fileInput = form.querySelector('.file-input');
    const quoteInput = form.querySelector('.quote-input');
    if (e.target.value == "quote") {
        fileInput.style.display = 'none';
        quoteInput.style.display = '';
        
        fileInput.required = false;
        quoteInput.required = true;
    } else {
        fileInput.style.display = '';
        fileInput.accept = e.target.value + '/*';
        quoteInput.style.display = 'none';

        fileInput.required = true;
        quoteInput.required = false;
    }
}


function toggleForm(e) {
    if (e.target.innerHTML == "Create") {
        document.getElementById('new-question-form').style.display = '';
        e.target.innerHTML = "Cancel";
    } else {
        document.getElementById('new-question-form').style.display = 'none';
        e.target.innerHTML = "Create";
    }
}

function toggleEditForm(e) {
    const id = e.target.id.substring("question-edit-toggle-".length);
    const prefix = 'edit-form-'
    if (e.target.innerHTML == "Edit") {
        document.getElementById(prefix + id).style.display = '';
        e.target.innerHTML = "Cancel";
    } else {
        document.getElementById(prefix + id).style.display = 'none';
        e.target.innerHTML = "Edit";
    }
}