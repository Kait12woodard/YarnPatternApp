const list = document.getElementById('patternList');
const empty = document.getElementById('emptyState');

fetch('/Home/GetPatterns')
    .then(response => response.json())
    .then(patterns => {
        if (patterns.length === 0) {
            list.hidden = true;
            empty.hidden = false;
            return;
        }

        list.hidden = false;
        empty.hidden = true;

        patterns.forEach(pattern => {
            const li = document.createElement('li');
            li.className = 'patterns';

            const fileName = pattern.filePath.split('/').pop();

            li.innerHTML = `
                <img src="/Home/GetThumbnail?fileName=${fileName}" style="width: 150px; height: 150px; object-fit: cover;">
                <h3>${pattern.name}</h3>
                <p>${pattern.craftType}</p>
                <a href="/Home/ViewPatternPdf?fileName=${fileName}" target="_blank">View PDF</a>
            `;

            list.appendChild(li);
        });
    });