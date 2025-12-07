import { Box, Button, Typography, Stack } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function AdminHome() {
  const navigate = useNavigate();

  return (
    <Box sx={{ maxWidth: 600, mx: "auto", mt: 5 }}>
      <Typography variant="h4" sx={{ mb: 3 }}>
        🔐 Admin Dashboard
      </Typography>

      <Stack spacing={2}>
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
