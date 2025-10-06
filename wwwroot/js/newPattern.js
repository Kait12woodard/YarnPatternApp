document.addEventListener('DOMContentLoaded', function (e) {

    const form = document.querySelector('form');
    if (!form) return;

    if (document.getElementById('projectTypesInput')) {
        initTagInput('projectTypesInput', 'projectTypesDisplay', 'ProjectTypes');
    }
    if (document.getElementById('yarnWeightsInput')) {
        initTagInput('yarnWeightsInput', 'yarnWeightsDisplay', 'YarnWeights', validateYarnWeight);
    }
    if (document.getElementById('toolSizesInput')) {
        initTagInput('toolSizesInput', 'toolSizesDisplay', 'ToolSizes', validateToolSize);
    }
    if (document.getElementById('yarnBrandsInput')) {
        initTagInput('yarnBrandsInput', 'yarnBrandsDisplay', 'YarnBrands');
    }
    if (document.getElementById('tagsInput')) {
        initTagInput('tagsInput', 'tagsDisplay', 'Tags');
    }

    form.addEventListener('submit', function (e) {
        createHiddenInputsFromTags('projectTypesDisplay', 'ProjectTypes');
        createHiddenInputsFromTags('yarnWeightsDisplay', 'YarnWeights');
        createHiddenInputsFromTags('toolSizesDisplay', 'ToolSizes');
        createHiddenInputsFromTags('yarnBrandsDisplay', 'YarnBrands');
        createHiddenInputsFromTags('tagsDisplay', 'Tags');
    });

    document.getElementById('parsePdfBtn')?.addEventListener('click', async function () {
        const fileInput = document.getElementById('pdfUpload');
        const statusDiv = document.getElementById('parseStatus');

        if (!fileInput.files[0]) {
            statusDiv.innerHTML = '<div class="alert alert-warning">Please select a PDF file first.</div>';
            return;
        }

        clearFormFields();

        statusDiv.innerHTML = '<div class="alert alert-info">Parsing PDF... Please wait.</div>';

        const formData = new FormData();
        formData.append('pdfFile', fileInput.files[0]);

        try {
            const response = await fetch('/Home/ParsePdf', {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                if (result.data.name) document.querySelector('input[name="Name"]').value = result.data.name;
                if (result.data.designer) document.querySelector('input[name="Designer"]').value = result.data.designer;
                if (result.data.craftType) document.querySelector('input[name="CraftType"]').value = result.data.craftType;
                if (result.data.difficulty) document.querySelector('select[name="Difficulty"]').value = result.data.difficulty;
                if (result.data.patSource) document.querySelector('input[name="PatSource"]').value = result.data.patSource;
                if (result.data.filePath) document.querySelector('input[name="FilePath"]').value = result.data.filePath;

                result.data.yarnWeights?.forEach(weight => {
                    addTagToInput('yarnWeightsInput', 'yarnWeightsDisplay', weight, validateYarnWeight);
                });
                result.data.toolSizes?.forEach(size => {
                    addTagToInput('toolSizesInput', 'toolSizesDisplay', size, validateToolSize);
                });
                result.data.projectTypes?.forEach(type => {
                    addTagToInput('projectTypesInput', 'projectTypesDisplay', type);
                });
                result.data.yarnBrands?.forEach(brand => {
                    addTagToInput('yarnBrandsInput', 'yarnBrandsDisplay', brand);
                });
                result.data.tags?.forEach(tag => {
                    addTagToInput('tagsInput', 'tagsDisplay', tag);
                });

                statusDiv.innerHTML = `<div class="alert alert-success">${result.message}</div>`;
                document.querySelector('.card-header').scrollIntoView({ behavior: 'smooth' });

            } else {
                statusDiv.innerHTML = '<div class="alert alert-danger">Failed to parse PDF. Please fill out manually.</div>';
            }

        } catch (error) {
            console.error('PDF parsing error:', error);
            statusDiv.innerHTML = '<div class="alert alert-danger">Error parsing PDF. Please try again or fill out manually.</div>';
        }
    });

    function clearFormFields() {
        const nameInput = document.querySelector('input[name="Name"]');
        const designerInput = document.querySelector('input[name="Designer"]');
        const craftTypeInput = document.querySelector('input[name="CraftType"]');
        const patSourceInput = document.querySelector('input[name="PatSource"]');
        const filePathInput = document.querySelector('input[name="FilePath"]');

        if (nameInput) nameInput.value = '';
        if (designerInput) designerInput.value = '';
        if (craftTypeInput) craftTypeInput.value = '';
        if (patSourceInput) patSourceInput.value = '';
        if (filePathInput) filePathInput.value = '';

        const difficultySelect = document.querySelector('select[name="Difficulty"]');
        if (difficultySelect) difficultySelect.value = '';

        const isFreeCheckbox = document.querySelector('input[name="IsFree"]');
        const isFavoriteCheckbox = document.querySelector('input[name="IsFavorite"]');
        const haveMadeCheckbox = document.querySelector('input[name="HaveMade"]');

        if (isFreeCheckbox) isFreeCheckbox.checked = false;
        if (isFavoriteCheckbox) isFavoriteCheckbox.checked = false;
        if (haveMadeCheckbox) haveMadeCheckbox.checked = false;

        clearTagDisplay('projectTypesDisplay');
        clearTagDisplay('yarnWeightsDisplay');
        clearTagDisplay('toolSizesDisplay');
        clearTagDisplay('yarnBrandsDisplay');
        clearTagDisplay('tagsDisplay');
    }

    function clearTagDisplay(displayId) {
        const display = document.getElementById(displayId);
        if (display) {
            display.innerHTML = '';
        }
    }

    function addTagToInput(inputId, displayId, value, validator) {
        const input = document.getElementById(inputId);
        const display = document.getElementById(displayId);
        if (input && display) {
            input.value = value;
            addTag(input, display, validator);
        }
    }
    function initTagInput(inputId, displayId, fieldName, validator) {
        const input = document.getElementById(inputId);
        const display = document.getElementById(displayId);
        if (!input || !display) return;

        const container = input.closest('.tag-input-container');
        if (!container) return;

        container.addEventListener('click', function () {
            input.focus();
        });

        input.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ',') {
                e.preventDefault();
                e.stopPropagation();
                addTag(input, display, validator);
            }
        });

        input.addEventListener('blur', function () {
            setTimeout(function () {
                if (input.value.trim()) {
                    addTag(input, display, validator);
                }
            }, 100);
        });
    }

    function addTag(input, display, validator) {
        const value = input.value.trim();
        if (!value) return;

        if (validator && !validator(value)) {
            input.classList.add('is-invalid');
            setTimeout(function () {
                input.classList.remove('is-invalid');
            }, 2000);
            return;
        }

        const existing = Array.from(display.children).map(function (pill) {
            return pill.textContent.replace('×', '').trim();
        });

        if (existing.includes(value)) {
            input.value = '';
            return;
        }

        const pill = document.createElement('div');
        pill.className = 'tag-pill';
        pill.innerHTML = value + ' <button type="button" class="tag-remove">×</button>';

        const removeBtn = pill.querySelector('.tag-remove');
        removeBtn.addEventListener('click', function () {
            pill.remove();
        });

        display.appendChild(pill);
        input.value = '';
    }

    function validateYarnWeight(value) {
        const num = parseInt(value);
        return !isNaN(num) && num >= 0 && num <= 7;
    }

    function validateToolSize(value) {
        const num = parseFloat(value);
        return !isNaN(num) && num > 0;
    }

    function createHiddenInputsFromTags(displayId, fieldName) {
        const display = document.getElementById(displayId);
        if (!display) return;

        const tags = Array.from(display.children).map(function (pill) {
            return pill.textContent.replace('×', '').trim();
        });

        tags.forEach(function (tag, index) {
            const hiddenInput = document.createElement('input');
            hiddenInput.type = 'hidden';
            hiddenInput.name = fieldName + '[' + index + ']';
            hiddenInput.value = tag;
            form.appendChild(hiddenInput);
        });
    }

}); 