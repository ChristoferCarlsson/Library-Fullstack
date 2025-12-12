import { Box, Button, Typography, Stack } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function AdminHome() {
  const navigate = useNavigate();

  return (
    <Box sx={{ width: "100%", maxWidth: 600, mx: "auto", mt: 3, mb: 4 }}>
      <Typography variant="h4" className="page-title">
        🔐 Admin Dashboard
      </Typography>

      <Stack spacing={2} sx={{ mt: 2 }}>
        <Button variant="contained" onClick={() => navigate("/admin/books")}>
          Manage Books
        </Button>

        <Button variant="contained" onClick={() => navigate("/admin/authors")}>
          Manage Authors
        </Button>

        <Button variant="contained" onClick={() => navigate("/admin/members")}>
          View Members & Loans
        </Button>
      </Stack>
    </Box>
  );
}
