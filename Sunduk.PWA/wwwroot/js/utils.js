window.triggerAutoGrow = () => {
    document.querySelectorAll('textarea').forEach(t => t.dispatchEvent(new Event('input')));
};