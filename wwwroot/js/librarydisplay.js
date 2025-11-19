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
            const col = document.createElement('div');
            col.className = 'col-12 col-sm-6 col-md-4 col-lg-3';
            const fileName = pattern.filePath.split('/').pop();
            col.innerHTML = `
                <div class="card h-100 shadow-sm">
                    <div class="card-body text-center">
                        <h5 class="card-title mb-3">${pattern.name}</h5>
                        <img src="/Home/GetThumbnail?fileName=${fileName}" 
                             class="img-fluid rounded mb-3" 
                             alt="${pattern.name}"
                             style="width: 100%; height: 200px; object-fit: cover;">
                    </div>
                    <div class="card-footer bg-white border-0 text-center pb-3">
                        <a href="/Home/ViewPatternPdf?fileName=${fileName}" 
                           target="_blank" 
                           class="btn btn-outline-primary btn-sm me-2">
                            View PDF
                        </a>
                        <a href="/Home/EditPattern?id=${pattern.id}"
                            class="btn btn-outline-secondary btn-sm">
                            Edit Pattern
                        </a>
                    </div>
                </div>
            `;
            list.appendChild(col);
        });
    });