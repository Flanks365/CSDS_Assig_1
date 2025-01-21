// // Async function to fetch data from the server
async function fetchData() {
    try {
        const response = await fetch(window.location.origin + '/categoriesFromDB');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data);
        return data.length > 0 ? data : []; // Return an empty array if no data found
    } catch (error) {
        console.error('Error fetching data:', error);
        return []; 
    }
}

// Helper function to generate form
function createCategoryForm(category, autoplay = false) {
    const form = document.createElement("form");
    form.style.border = "0px";
    form.action = "Quizpage";
    form.method = "get";

    const hiddenCategoryName = document.createElement("input");
    hiddenCategoryName.type = "hidden";
    hiddenCategoryName.name = "category_name";
    hiddenCategoryName.value = category.name;

    const hiddenCategoryId = document.createElement("input");
    hiddenCategoryId.type = "hidden";
    hiddenCategoryId.name = "category_id";
    hiddenCategoryId.value = category.id;

    const hiddenAutoplay = document.createElement("input");
    hiddenAutoplay.type = "hidden";
    hiddenAutoplay.name = "autoplay";
    hiddenAutoplay.value = autoplay ? "true" : "false";

    const submitButton = document.createElement("input");
    submitButton.type = "submit";
    submitButton.value = autoplay ? "Autoplay Quiz" : "Play Quiz";

    form.appendChild(hiddenCategoryName);
    form.appendChild(hiddenCategoryId);
    form.appendChild(hiddenAutoplay);
    form.appendChild(submitButton);

    return form;
}

async function generateCategoryCards() {
    const categoriesInfo = await fetchData();
    const categoryContainer = document.getElementById("categoriesContainer");

    if (categoriesInfo.length === 0) {
        categoryContainer.innerText = "No categories available.";
        return;
    }

    categoriesInfo.forEach(category => {
        const categoryDiv = document.createElement("div");
        categoryDiv.className = "categoryContainer";
        categoryDiv.style.display = "flex";

        const categoryName = document.createElement("h3");
        categoryName.textContent = category.name;
        categoryDiv.appendChild(categoryName);

        const categoryImage = document.createElement("img");
        categoryImage.className = "categoryImg";
        categoryImage.src = `data:${category.imageType};base64,${category.image}`;
        categoryDiv.appendChild(categoryImage);

        // Add both Play Quiz and Autoplay Quiz forms
        categoryDiv.appendChild(createCategoryForm(category, false)); // Play Quiz
        categoryDiv.appendChild(createCategoryForm(category, true));  // Autoplay Quiz

        categoryContainer.appendChild(categoryDiv);
    });
}

// Call the function to fetch data and generate the category cards
generateCategoryCards();
