class UserInfoLayout {
    constructor() {
        this.userInfoDiv = document.getElementById('userInfo');
        this.userDiv = document.getElementById('userDiv');
        this.hrElement = null;

        // Attach resize event listener
        window.addEventListener('resize', this.checkAndInsertHr.bind(this));

        // Call the function initially to set up the correct state
        this.checkAndInsertHr();
    }

    checkAndInsertHr() {
        if (window.innerWidth < 991) {
            this.applySmallScreenLayout();
        } else {
            this.applyLargeScreenLayout();
        }
    }

    applySmallScreenLayout() {
        this.userInfoDiv.classList.remove('mx-5');
        this.userDiv.classList.add('text-center');

        if (!this.hrElement) {
            this.hrElement = document.createElement('hr');
            this.hrElement.id = 'hrUserInfo';
            this.userInfoDiv.insertAdjacentElement('afterbegin', this.hrElement);
        }
    }

    applyLargeScreenLayout() {
        this.userInfoDiv.classList.add('mx-5');
        this.userDiv.classList.remove('text-center');

        if (this.hrElement) {
            this.hrElement.remove();
            this.hrElement = null;
        }
    }
}

// Usage
document.addEventListener('DOMContentLoaded', () => {
    if (!location.href.includes('admin') && !location.href.includes('login')){
        new UserInfoLayout();
    }
});
