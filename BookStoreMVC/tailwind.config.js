/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ['./Views/*/**.cshtml'],
  theme: {
      extend: {
          colors: {
              primary: '#34979D',
              titleText: '#4D4D4D',
              normalText: '#8B8B8B',
              fade: '#C5C5C5',
          },
          fontFamily: {
              inter: ['Inter'],
              glory: ['Glory']
          },
      },
  },
  plugins: [],
}
