/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./Views/*/**.cshtml"],
  mode: "jit",
  theme: {
    extend: {
      colors: {
        primary: "#34979D",
        primary2: {
          100: "#d6eaeb",
          200: "#aed5d8",
          300: "#85c1c4",
          400: "#5dacb1",
          default: "#34979d",
          600: "#2a797e",
          700: "#1f5b5e",
          800: "#153c3f",
          900: "#0a1e1f",
        },
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
  plugins: [require("@tailwindcss/line-clamp"), require("tailwind-scrollbar")],
};
