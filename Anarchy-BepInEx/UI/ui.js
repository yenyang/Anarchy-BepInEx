// Function to set up Anarchy Button
if (typeof setupAnarchyItemYYA !== 'function')
{
    function setupAnarchyItemYYA()
    {
        const button = document.getElementById("YYA-Anarchy-Button");
        if (button == null) {
            engine.trigger('YYA-log', "JS Error: could not setup button YYA-Anarchy-Button");
            return;
        }
        if (anarchyEnabledYYA) {
            button.classList.add("selected");
            const image = document.getElementById("YYA-Anarchy-Image");
            if (image != null) {
                image.src = "coui://uil/Colored/Anarchy.svg";
            }
            if (flamingChirperYYA) {
                var y = document.getElementsByTagName("img");
                for (let i = 0; i < y.length; i++) {
                    if (y[i].src == "coui://GameUI/Media/Game/Icons/Chirper.svg" || y[i].src == "Media/Game/Icons/Chirper.svg") y[i].src = "coui://uil/Colored/AnarchyChirper.svg";
                }
            }
        } else {
            const image = document.getElementById("YYA-Anarchy-Image");
            if (image != null) {
                image.src = "coui://uil/Standard/Anarchy.svg";
            }
            button.classList.remove("selected");
            var y = document.getElementsByTagName("img");
            for (let i = 0; i < y.length; i++) {
                if (y[i].src == "coui://uil/Colored/AnarchyChirper.svg") y[i].src = "coui://GameUI/Media/Game/Icons/Chirper.svg";
            }
        }
        button.onclick = function () {
            anarchyEnabledYYA = !anarchyEnabledYYA;
            engine.trigger('YYA-AnarchyToggled', anarchyEnabledYYA);
            const thisButton = document.getElementById(this.id);
            if (anarchyEnabledYYA) {
                thisButton.classList.add("selected");
                const image = document.getElementById("YYA-Anarchy-Image");
                if (image != null) {
                    image.src = "coui://uil/Colored/Anarchy.svg";
                }
                if (flamingChirperYYA) {
                    var y = document.getElementsByTagName("img");
                    for (let i = 0; i < y.length; i++) {
                        if (y[i].src == "coui://GameUI/Media/Game/Icons/Chirper.svg" || y[i].src == "Media/Game/Icons/Chirper.svg") y[i].src = "coui://uil/Colored/AnarchyChirper.svg";
                    }
                }
            } else {
                const image = document.getElementById("YYA-Anarchy-Image");
                if (image != null) {
                    image.src = "coui://uil/Standard/Anarchy.svg";
                }
                thisButton.classList.remove("selected");
                var y = document.getElementsByTagName("img");
                for (let i = 0; i < y.length; i++) {
                    if (y[i].src == "coui://uil/Colored/AnarchyChirper.svg") y[i].src = "coui://GameUI/Media/Game/Icons/Chirper.svg";
                }
            }
            
        }
    }
}

if (typeof CheckForElementByID !== 'function') {
    function CheckForElementByID(id)
    {
        if (document.getElementById(id) != null) {
            engine.trigger('CheckForElement-'+id , true);
            return;
        }
        engine.trigger('CheckForElement-' + id, false);
    }
}

if (typeof setupButton !== 'function') {
    function setupButton(buttonId, selected) {
        const button = document.getElementById(buttonId);
        if (button == null) {
            engine.trigger('YYA-log', "JS Error: could not setup button " + buttonId);
            return;
        }
        if (selected) {
            button.classList.add("selected");
        } else {
            button.classList.remove("selected");
        }
        button.onclick = function () {
            let selectedYYA = true;
            if (this.classList.contains("selected")) {
                selectedYYA = false; // This is intended to toggle and be the opposite of what it is now.
            }
            engine.trigger(this.id, selectedYYA);
            const thisButton = document.getElementById(this.id);
            if (selectedYYA) {
                thisButton.classList.add("selected");
            } else {
                thisButton.classList.remove("selected");
            }

        }
    }
}

