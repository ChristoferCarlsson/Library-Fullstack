import { Box, Typography, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function Logo() {
  const navigate = useNavigate();

  return (
    <Box
      sx={{
        width: "100%",
        maxWidth: 1100,
        mx: "auto",
        mt: 1,
        mb: 3,
        px: 3,
        py: 2,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        position: "relative",
        backgroundColor: "rgba(255,255,255,0.6)",
        backdropFilter: "blur(6px)",
        borderRadius: 2,
        boxShadow: "0 2px 6px rgba(0,0,0,0.08)",
      }}
    >
      <Typography
        variant="h4"
        sx={{
          fontWeight: "bold",
          cursor: "pointer",
          transition: "transform 0.25s ease, color 0.25s ease",
          "&:hover": {
            transform: "scale(1.06)",
            color: "#6b4f4f",
            textShadow: "0 2px 6px rgba(0,0,0,0.15)",
          },
        }}
        onClick={() => navigate("/")}
      >
        Full Stack Library
      </Typography>

      <Button
        variant="contained"
        color="secondary"
        onClick={() => navigate("/admin")}
        sx={{ position: "absolute", right: 20 }}
      >
        Admin
      </Button>
    </Box>
  );
}
