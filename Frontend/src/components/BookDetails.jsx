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
import { validate, hasErrors } from "../utils/validate.js";

export default function BookDetails({ onDataChanged }) {
  const { id } = useParams();
  const [book, setBook] = useState(null);
  const [loan, setLoan] = useState(null);
  const [apiError, setApiError] = useState("");

  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ firstName: "", lastName: "", email: "" });
  const [fieldErrors, setFieldErrors] = useState({});

  const notifyChanged = () => {
    if (typeof onDataChanged === "function") onDataChanged();
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setFieldErrors((prev) => ({ ...prev, [name]: "" }));
  };

  const loadBook = async () => {
    try {
      // 1. Load book details
      const bookRes = await api.get(`/books/${id}`);
      setBook(bookRes.data);

      // 2. Load all loans
      const loansRes = await api.get("/loans");

      // 3. Find ACTIVE loan for THIS book
      const activeLoan = loansRes.data.find(
        (l) => l.bookId === Number(id) && !l.returnDate
      );

      setLoan(activeLoan || null);
    } catch (err) {
      console.error("Failed to load book details", err);
      setApiError("Failed to load book details.");
    }
  };

  useEffect(() => {
    loadBook();
  }, [id]);

  const validateLoanForm = (currentForm) => {
    const errors = validate(currentForm, {
      firstName: { required: true, label: "first name" },
      lastName: { required: true, label: "last name" },
      email: {
        required: true,
        label: "email",
        email: true,
        emailMessage: "Please enter a valid email address",
      },
    });

    setFieldErrors(errors);
    return !hasErrors(errors);
  };

  const loanBook = async () => {
    try {
      setApiError("");

      if (!validateLoanForm(form)) return;

      const email = form.email.trim().toLowerCase();

      // Find existing member OR create a new one
      const membersRes = await api.get("/members");
      let member = membersRes.data.find((m) => m.email.toLowerCase() === email);

      if (!member) {
        const created = await api.post("/members", {
          firstName: form.firstName.trim(),
          lastName: form.lastName.trim(),
          email,
        });
        member = created.data;
      }

      // Create the loan
      await api.post("/Loans", {
        bookId: Number(id),
        memberId: member.id,
        dueDate: new Date(Date.now() + 12096e5).toISOString(), // 14 days
      });

      setOpen(false);
      setForm({ firstName: "", lastName: "", email: "" });
      setFieldErrors({});
      await loadBook();
      notifyChanged();
    } catch (err) {
      console.error(err);
      setApiError(err.response?.data?.message || "Loan failed.");
    }
  };

  const returnBook = async () => {
    try {
      setApiError("");
      await api.put(`/Loans/${loan.id}/return`);
      await loadBook();
      notifyChanged();
    } catch (err) {
      console.error(err);
      setApiError(err.response?.data?.message || "Return failed.");
    }
  };

  if (!book) return <Typography>Loading...</Typography>;

  return (
    <Box sx={{ maxWidth: 800, mx: "auto", mt: 4 }}>
      {apiError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {apiError}
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
            {!loan && book.copiesAvailable > 0 && (
              <Button variant="contained" onClick={() => setOpen(true)}>
                Loan Book
              </Button>
            )}

            {loan && (
              <Button variant="outlined" color="secondary" onClick={returnBook}>
                Return Book
              </Button>
            )}
          </Stack>
        </CardContent>
      </Card>

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
              error={!!fieldErrors.firstName}
              helperText={fieldErrors.firstName}
            />

            <TextField
              label="Last Name"
              name="lastName"
              value={form.lastName}
              onChange={handleChange}
              fullWidth
              error={!!fieldErrors.lastName}
              helperText={fieldErrors.lastName}
            />

            <TextField
              type="email"
              label="Email"
              name="email"
              value={form.email}
              onChange={handleChange}
              fullWidth
              error={!!fieldErrors.email}
              helperText={fieldErrors.email}
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
