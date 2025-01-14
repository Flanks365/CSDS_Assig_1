// Async function to fetch data from the server
async function fetchData() {
    try {
        const response = await fetch('http://localhost/categoriesFromDB');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data);

        // Return the fetched data so it can be used elsewhere
        return data;
    } catch (error) {
        console.error('Error fetching data:', error);
        return []; // Return an empty array in case of error
    }
}

// Fetch the categories and generate the HTML
async function generateCategoryCards() {
    const categoriesInfo = await fetchData();

    // Get the container where the cards will be appended
    const categoryContainer = document.getElementById("categoriesContainer");

    // Check if categoriesInfo has data before trying to loop through it
    if (categoriesInfo.length === 0) {
        categoryContainer.innerText = "No categories available.";
        return;
    }

    // Generate and append cards for each category
    categoriesInfo.forEach(category => {
        // Create the card container
        const categoryDiv = document.createElement("div");
        categoryDiv.className = "categoryContainer";
        categoryDiv.style.display = "flex";

        // Add category name
        const categoryName = document.createElement("h3");
        categoryName.textContent = category.name;
        categoryDiv.appendChild(categoryName);

        // Add category image
        const categoryImage = document.createElement("img");
        categoryImage.className = "categoryImg";
        categoryImage.src = `data:${category.imageType};base64,${category.image}`;
        categoryDiv.appendChild(categoryImage);

        // Create the first form (Play Quiz)
        const formPlay = document.createElement("form");
        formPlay.style.border = "0px";
        formPlay.action = "Quizpage";
        formPlay.method = "get";

        const hiddenCategoryName1 = document.createElement("input");
        hiddenCategoryName1.type = "hidden";
        hiddenCategoryName1.name = "category_name";
        hiddenCategoryName1.value = category.name;

        const hiddenAutoplay1 = document.createElement("input");
        hiddenAutoplay1.type = "hidden";
        hiddenAutoplay1.name = "autoplay";
        hiddenAutoplay1.value = "false";

        const submitPlay = document.createElement("input");
        submitPlay.type = "submit";
        submitPlay.value = "Play Quiz";

        formPlay.appendChild(hiddenCategoryName1);
        formPlay.appendChild(hiddenAutoplay1);
        formPlay.appendChild(submitPlay);

        categoryDiv.appendChild(formPlay);

        // Create the second form (Autoplay Quiz)
        const formAutoplay = document.createElement("form");
        formAutoplay.style.border = "0px";
        formAutoplay.action = "Quizpage";
        formAutoplay.method = "get";

        const hiddenCategoryName2 = document.createElement("input");
        hiddenCategoryName2.type = "hidden";
        hiddenCategoryName2.name = "category_name";
        hiddenCategoryName2.value = category.name;

        const hiddenAutoplay2 = document.createElement("input");
        hiddenAutoplay2.type = "hidden";
        hiddenAutoplay2.name = "autoplay";
        hiddenAutoplay2.value = "true";

        const submitAutoplay = document.createElement("input");
        submitAutoplay.type = "submit";
        submitAutoplay.value = "Autoplay Quiz";

        formAutoplay.appendChild(hiddenCategoryName2);
        formAutoplay.appendChild(hiddenAutoplay2);
        formAutoplay.appendChild(submitAutoplay);

        categoryDiv.appendChild(formAutoplay);

        // Append the categoryDiv to the main container
        categoryContainer.appendChild(categoryDiv);
    });
}

// Call the function to fetch data and generate the category cards
generateCategoryCards();
