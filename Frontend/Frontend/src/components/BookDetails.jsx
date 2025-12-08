import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  Stack,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
} from "@mui/material";
import api from "../api/AxiosClient";

export default function BookDetails() {
  const { id } = useParams();
  const [book, setBook] = useState(null);
  const [loan, setLoan] = useState(null);
  const [error, setError] = useState("");

  // Modal state
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
  });

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function loadBook() {
    const res = await api.get(`/books/${id}`);
    setBook(res.data);

    // Load ALL active loans and see if this user already has a loan for this book
    const loanRes = await api.get(`/loans`);
    const activeLoan = loanRes.data.find(
      (l) => l.bookId === Number(id) && !l.returnDate
    );
    setLoan(activeLoan || null);
  }

  useEffect(() => {
    loadBook();
  }, []);

  // --------------------------------------------------------------
  // Loan book logic (create or reuse existing member)
  // --------------------------------------------------------------
  async function loanBook() {
    try {
      setError("");

      // 1. Try to find existing member by email
      const members = await api.get("/members");
      let member = members.data.find((m) => m.email === form.email);

      // 2. If no existing member → create one
      if (!member) {
        const created = await api.post("/members", {
          firstName: form.firstName,
          lastName: form.lastName,
          email: form.email,
        });
        member = created.data;
      }

      // 3. Create the loan
      await api.post("/loans", {
        bookId: Number(id),
        memberId: member.id,
        dueDate: new Date(Date.now() + 12096e5).toISOString(), // 14 days
      });

      setOpen(false);
      loadBook();
    } catch (err) {
      console.error(err);
      setError(err.response?.data?.message || "Loan failed.");
    }
  }

  async function returnBook() {
    try {
      setError("");
      await api.put(`/loans/${loan.id}/return`);
      loadBook();
    } catch (err) {
      console.error(err);
      setError(err.response?.data?.message || "Return failed.");
    }
  }

  if (!book) return <Typography>Loading...</Typography>;

  return (
    <Box sx={{ maxWidth: 800, mx: "auto", mt: 4 }}>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Card>
        <CardContent>
          <Typography variant="h4">{book.title}</Typography>
          <Typography variant="h6" sx={{ mt: 1 }}>
            Author: {book.authorFullName}
          </Typography>

          <Typography sx={{ mt: 1 }}>
            Copies Available: {book.copiesAvailable}/{book.copiesTotal}
          </Typography>

          <Stack direction="row" spacing={2} sx={{ mt: 3 }}>
            {/* Loan Button */}
            {book.copiesAvailable > 0 && !loan && (
              <Button variant="contained" onClick={() => setOpen(true)}>
                Loan Book
              </Button>
            )}

            {/* Return Button */}
            {loan && (
              <Button color="secondary" variant="outlined" onClick={returnBook}>
                Return Book
              </Button>
            )}
          </Stack>
        </CardContent>
      </Card>

      {/* -------------------------------------------------------
           LOAN MODAL
      -------------------------------------------------------- */}
      <Dialog open={open} onClose={() => setOpen(false)} fullWidth>
        <DialogTitle>Loan This Book</DialogTitle>

        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="First Name"
              name="firstName"
              value={form.firstName}
              onChange={handleChange}
              fullWidth
            />

            <TextField
              label="Last Name"
              name="lastName"
              value={form.lastName}
              onChange={handleChange}
              fullWidth
            />

            <TextField
              type="email"
              label="Email"
              name="email"
              value={form.email}
              onChange={handleChange}
              fullWidth
            />
          </Stack>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={loanBook}>
            Loan Book
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
