/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml', // Inclua todos os Razor Pages
        './Views/**/*.cshtml', // Inclua as Views se estiver usando MVC
        './wwwroot/**/*.html', // Qualquer HTML dentro do wwwroot
        './wwwroot/**/*.js'    // Scripts JS no wwwroot
    ],
    theme: {
        extend: {},
    },
    plugins: [],
};
