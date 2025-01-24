document.getElementById('loginForm').addEventListener('submit', checkLogin);

async function checkLogin(e) {
    e.preventDefault(); // Prevent default form submission

    // Collect form data
    const formData = {
        username: document.getElementById('username').value,
        password: document.getElementById('password').value,
    };

    try {
        // Send data using AJAX
        const response = await fetch('/loginPost', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Specify JSON format
            },
            body: JSON.stringify(formData), // Convert form data to JSON
        });

        // Handle response
        if (response.ok) {
            window.location.href = '/main';
        } else {
            const errorText = await response.text();
            document.getElementById("errorMsg").innerHTML = "Invalid username or password";
        }
    } catch (error) {
        console.error("Error occurred:", error);
        document.getElementById("errorMsg").innerHTML = "An error occurred. Please try again.";
    }
}
