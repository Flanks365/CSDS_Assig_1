// // Async function to post data to the server
async function handleSignup(event) {
    event.preventDefault(); // Prevent the form from submitting the traditional way

    const username = document.getElementById("user_id").value;
    const password = document.getElementById("password").value;

    // Create the signup data object
    const signupData = {
        username: username,
        password: password
    };

    // Send the signup data as JSON using the fetch API
    const response = await fetch('/signup', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(signupData)
    });

    if (response.ok) {
        // Redirect or handle successful signup
        window.location.href = '/main';

    } else {
        const result = await response.json();
        alert(result.message || "Signup failed!");
    }
}