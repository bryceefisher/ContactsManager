document.addEventListener('DOMContentLoaded', () => {
    
    const userInfoDiv = document.getElementById('userInfo');
    const userDiv = document.getElementById('userDiv');

    const checkAndInsertHr = () => {
        // Check if the hr already exists
        let hrElement = document.getElementById('hrUserInfo');

        if (window.innerWidth < 991) {
            userInfoDiv.classList.remove('mx-5');
            userDiv.classList.add('text-center');
            if (!hrElement) {
                // If the hr doesn't exist and screen is less than 1200px, create and insert it
                hrElement = document.createElement('hr');
                hrElement.id = 'hrUserInfo';
                // Insert the hr element after the userInfo div
                userInfoDiv.insertAdjacentElement('afterbegin', hrElement);
            }
        } else {
            userInfoDiv.classList.add('mx-5');
            userDiv.classList.remove('text-center');
            // If screen is not less than 1200px and hr exists, remove it
            if (hrElement) {
                hrElement.remove();
                
            }
        }
    };

    // Attach the function to the resize event listener
    window.addEventListener('resize', checkAndInsertHr);

    // Call the function initially to set up the correct state
    checkAndInsertHr();
});
