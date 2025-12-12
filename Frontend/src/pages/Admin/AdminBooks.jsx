import { useEffect, useState } from "react";
import {
  Box,
  Button,
  Typography,
  Card,
  CardContent,
  Stack,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
} from "@mui/material";
import api from "../../api/AxiosClient";
import { validate, hasErrors } from "../../utils/validate.js";

export default function AdminBooks({ onDataChanged }) {
  const [books, setBooks] = useState([]);
  const [authors, setAuthors] = useState([]);

  const [open, setOpen] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [apiError, setApiError] = useState("");
  const [fieldErrors, setFieldErrors] = useState({});

  const today = new Date().toISOString().slice(0, 10);

  const emptyForm = {
    id: null,
    title: "",
    isbn: "",
    authorId: "",
    publishedDate: today,
    copiesTotal: 1,
  };

  const [form, setForm] = useState(emptyForm);

  const notifyChanged = () => {
    if (typeof onDataChanged === "function") onDataChanged();
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setFieldErrors((prev) => ({ ...prev, [name]: "" }));
  };

  const resetModal = (isEdit = false, values = emptyForm) => {
    setEditMode(isEdit);
    setApiError("");
    setFieldErrors({});
    setForm(values);
    setOpen(true);
  };

  const handleApiError = (err) => {
    const res = err.response?.data;

    if (res?.errors) {
      const key = Object.keys(res.errors)[0];
      setApiError(res.errors[key][0]);
    } else {
      setApiError(res?.message || "An unexpected error occurred.");
    }
  };

  const loadBooks = async () => {
    const res = await api.get("/books");
    setBooks(res.data);
  };

  const loadAuthors = async () => {
    const res = await api.get("/authors");
    setAuthors(res.data);
  };

  useEffect(() => {
    loadBooks();
    loadAuthors();
  }, []);

  const validateBookForm = (data) => {
    const errors = validate(data, {
      title: { required: true, label: "Title" },
      isbn: { required: true, label: "ISBN" },
      authorId: { required: true, requiredMessage: "Please select an author" },
      publishedDate: { required: true, label: "published date" },
      copiesTotal: {
        required: true,
        label: "total copies",
        min: 1,
        minMessage: "Total copies must be at least 1",
      },
    });

    setFieldErrors(errors);
    return !hasErrors(errors);
  };

  const createBook = async () => {
    if (!validateBookForm(form)) return;

    try {
      await api.post("/books", {
        title: form.title.trim(),
        isbn: form.isbn.trim(),
        authorId: Number(form.authorId),
        publishedDate: form.publishedDate || today,
        copiesTotal: Number(form.copiesTotal),
      });

      setOpen(false);
      await loadBooks();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  const updateBook = async () => {
    if (!validateBookForm(form)) return;

    try {
      await api.put(`/books/${form.id}`, {
        title: form.title.trim(),
        isbn: form.isbn.trim(),
        authorId: Number(form.authorId),
        publishedDate: form.publishedDate,
        copiesTotal: Number(form.copiesTotal),
      });

      setOpen(false);
      await loadBooks();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  const openCreateModal = () => resetModal(false);

  const openEditModal = (book) =>
    resetModal(true, {
      id: book.id,
      title: book.title,
      isbn: book.isbn,
      authorId: book.authorId,
      publishedDate: book.publishedDate?.slice(0, 10) || today,
      copiesTotal: book.copiesTotal,
    });

  const deleteBook = async (id) => {
    if (!window.confirm("Delete this book?")) return;
    setApiError("");

    try {
      await api.delete(`/books/${id}`);
      await loadBooks();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  return (
    <Box sx={{ width: "100%", maxWidth: 900, mx: "auto", mt: 3, mb: 4 }}>
      <Typography variant="h4" className="page-title">
        📘 Manage Books
      </Typography>

      <Button variant="contained" sx={{ mb: 2 }} onClick={openCreateModal}>
        Add New Book
      </Button>

      {books.map((b) => (
        <Card
          key={b.id}
          sx={{ mb: 2, "&:hover": { backgroundColor: "#faf7f2" } }}
        >
          <CardContent>
            <Typography variant="h6">{b.title}</Typography>
            <Typography>Author: {b.authorFullName}</Typography>
            <Typography>ISBN: {b.isbn}</Typography>
            <Typography>
              Copies: {b.copiesAvailable}/{b.copiesTotal}
            </Typography>

            <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
              <Button variant="outlined" onClick={() => openEditModal(b)}>
                Edit
              </Button>
              <Button
                color="error"
                variant="outlined"
                onClick={() => deleteBook(b.id)}
              >
                Delete
              </Button>
            </Stack>
          </CardContent>
        </Card>
      ))}

      {/* Modal */}
      <Dialog
        open={open}
        onClose={() => setOpen(false)}
        fullWidth
        PaperProps={{ sx: { borderRadius: 3, backgroundColor: "#fffdf8" } }}
      >
        <DialogTitle>{editMode ? "Edit Book" : "Add Book"}</DialogTitle>

        <DialogContent>
          {apiError && (
            <Typography color="error" sx={{ mb: 2 }}>
              ⚠️ {apiError}
            </Typography>
          )}

          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="Title"
              name="title"
              value={form.title}
              onChange={handleChange}
              fullWidth
              error={!!fieldErrors.title}
              helperText={fieldErrors.title}
            />

            <TextField
              label="ISBN"
              name="isbn"
              value={form.isbn}
              onChange={handleChange}
              fullWidth
              error={!!fieldErrors.isbn}
              helperText={fieldErrors.isbn}
            />

            <TextField
              select
              label="Author"
              name="authorId"
              value={form.authorId}
              onChange={handleChange}
              fullWidth
              error={!!fieldErrors.authorId}
              helperText={fieldErrors.authorId}
            >
              <MenuItem value="">
                <em>Select an author…</em>
              </MenuItem>
              {authors.map((a) => (
                <MenuItem key={a.id} value={a.id}>
                  {a.fullName}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              type="date"
              label="Published Date"
              name="publishedDate"
              value={form.publishedDate}
              onChange={handleChange}
              fullWidth
              InputLabelProps={{ shrink: true }}
              error={!!fieldErrors.publishedDate}
              helperText={fieldErrors.publishedDate}
            />

            <TextField
              type="number"
              label="Copies Total"
              name="copiesTotal"
              value={form.copiesTotal}
              onChange={handleChange}
              inputProps={{ min: 1 }}
              fullWidth
              error={!!fieldErrors.copiesTotal}
              helperText={fieldErrors.copiesTotal}
            />
          </Stack>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={editMode ? updateBook : createBook}
          >
            {editMode ? "Update" : "Create"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
