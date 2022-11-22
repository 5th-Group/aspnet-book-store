/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./Views/*/**.cshtml"],
  mode: "jit",
  theme: {
    extend: {
      colors: {
        primary: "#34979D",
        titleText: "#4D4D4D",
        normalText: "#8B8B8B",
        fade: "#C5C5C5",
        backGr: "#FAFAFA",
        bf: "#FAFAFA",
      },
      fontFamily: {
        inter: ["Inter"],
        glory: ["Glory"],
      },
    },
  },
  plugins: [],
};
