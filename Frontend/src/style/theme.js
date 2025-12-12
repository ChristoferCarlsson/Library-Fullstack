import { createTheme } from "@mui/material/styles";

const theme = createTheme({
  palette: {
    mode: "light",
    background: {
      default: "#f7f4ee",
      paper: "#ffffff",
    },
    primary: {
      main: "#6b4f4f",
    },
    secondary: {
      main: "#a67c52",
    },
    text: {
      primary: "#3b2f2f",
      secondary: "#6b5e5e",
    },
  },

  typography: {
    fontFamily: `"Inter", system-ui, Avenir, Helvetica, Arial, sans-serif`,
    h4: { fontWeight: 700 },
    h6: { fontWeight: 600 },
  },

  shape: {
    borderRadius: 12,
  },

  components: {
    MuiPaper: {
      styleOverrides: {
        root: {
          padding: "1rem",
          borderRadius: 12,
          boxShadow: "0 2px 6px rgba(0,0,0,0.08)",
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 14,
          boxShadow: "0 3px 10px rgba(0,0,0,0.07)",
          transition: "transform 0.15s ease, box-shadow 0.15s ease",
        },
      },
    },
    MuiCardContent: {
      styleOverrides: {
        root: {
          padding: "1.2rem 1.5rem",
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 10,
          textTransform: "none",
          fontWeight: 600,
          padding: "0.55rem 1.4rem",
          boxShadow: "0 2px 5px rgba(0,0,0,0.15)",
        },
      },
    },
    MuiTextField: {
      styleOverrides: {
        root: {
          backgroundColor: "#ffffff",
          borderRadius: 8,
        },
      },
    },
  },
});

export default theme;
