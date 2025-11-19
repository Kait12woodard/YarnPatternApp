document.addEventListener('DOMContentLoaded', function () {
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

    prePopulateTags();

    form.addEventListener('submit', function (e) {
        createHiddenInputsFromTags('projectTypesDisplay', 'ProjectTypes');
        createHiddenInputsFromTags('yarnWeightsDisplay', 'YarnWeights');
        createHiddenInputsFromTags('toolSizesDisplay', 'ToolSizes');
        createHiddenInputsFromTags('yarnBrandsDisplay', 'YarnBrands');
        createHiddenInputsFromTags('tagsDisplay', 'Tags');
    });

    function prePopulateTags() {
        const projectTypes = getModelArrayData('ProjectTypes');
        const yarnWeights = getModelArrayData('YarnWeights');
        const toolSizes = getModelArrayData('ToolSizes');
        const yarnBrands = getModelArrayData('YarnBrands');
        const tags = getModelArrayData('Tags');

        projectTypes.forEach(item => addTagToDisplay('projectTypesDisplay', item));
        yarnWeights.forEach(item => addTagToDisplay('yarnWeightsDisplay', item));
        toolSizes.forEach(item => addTagToDisplay('toolSizesDisplay', item));
        yarnBrands.forEach(item => addTagToDisplay('yarnBrandsDisplay', item));
        tags.forEach(item => addTagToDisplay('tagsDisplay', item));
    }

    function getModelArrayData(fieldName) {
        const inputs = document.querySelectorAll(`input[name^="${fieldName}"]`);
        const values = [];
        inputs.forEach(input => {
            if (input.value) {
                values.push(input.value);
            }
        });
        return values;
    }

    function addTagToDisplay(displayId, value) {
        const display = document.getElementById(displayId);
        if (!display || !value) return;

        const pill = document.createElement('div');
        pill.className = 'tag-pill';
        pill.innerHTML = value + ' <button type="button" class="tag-remove">×</button>';

        const removeBtn = pill.querySelector('.tag-remove');
        removeBtn.addEventListener('click', function () {
            pill.remove();
        });

        display.appendChild(pill);
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

        addTagToDisplay(display.id, value);
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

        const oldInputs = form.querySelectorAll(`input[type="hidden"][name^="${fieldName}"]`);
        oldInputs.forEach(input => input.remove());

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