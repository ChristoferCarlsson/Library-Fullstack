import { Box, Typography, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function Logo() {
  const navigate = useNavigate();

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        px: 3,
        mt: 2,
      }}
    >
      <Typography
        variant="h4"
        sx={{ fontWeight: "bold", cursor: "pointer" }}
        onClick={() => navigate("/")}
      >
        📚 My Library
      </Typography>

      <Button
        variant="contained"
        color="secondary"
        onClick={() => navigate("/admin")}
      >
        Admin
      </Button>
    </Box>
  );
}
