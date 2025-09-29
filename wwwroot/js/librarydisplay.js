const list = document.getElementById('patternList');
const empty = document.getElementById('emptyState');

const items = list.querySelectorAll('li.patterns');
const hasItems = items.length > 0;

list.hidden = !hasItems;
empty.hidden = hasItems;
