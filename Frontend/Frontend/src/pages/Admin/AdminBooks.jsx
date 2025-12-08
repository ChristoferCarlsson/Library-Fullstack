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

export default function AdminBooks() {
  const [books, setBooks] = useState([]);
  const [authors, setAuthors] = useState([]);

  const [open, setOpen] = useState(false);
  const [editMode, setEditMode] = useState(false);

  const [error, setError] = useState("");

  const today = new Date().toISOString().slice(0, 10);

  const [form, setForm] = useState({
    id: null,
    title: "",
    isbn: "",
    authorId: "",
    publishedDate: today,
    copiesTotal: 1,
  });

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function loadBooks() {
    const res = await api.get("/books");
    setBooks(res.data);
  }

  async function loadAuthors() {
    const res = await api.get("/authors");
    setAuthors(res.data);
  }

  async function deleteBook(id) {
    if (!window.confirm("Delete this book?")) return;
    await api.delete(`/books/${id}`);
    loadBooks();
  }

  useEffect(() => {
    loadBooks();
    loadAuthors();
  }, []);

  // -----------------------------------------------------------
  // Create Modal
  // -----------------------------------------------------------
  function openCreateModal() {
    setEditMode(false);
    setError("");
    setForm({
      id: null,
      title: "",
      isbn: "",
      authorId: "",
      publishedDate: today,
      copiesTotal: 1,
    });
    setOpen(true);
  }

  async function createBook() {
    try {
      await api.post("/books", {
        title: form.title,
        isbn: form.isbn,
        authorId: Number(form.authorId),
        publishedDate: form.publishedDate || today,
        copiesTotal: Number(form.copiesTotal),
      });

      setOpen(false);
      loadBooks();
    } catch (err) {
      handleApiError(err);
    }
  }

  // -----------------------------------------------------------
  // Edit Modal
  // -----------------------------------------------------------
  function openEditModal(book) {
    setEditMode(true);
    setError("");
    setForm({
      id: book.id,
      title: book.title,
      isbn: book.isbn,
      authorId: book.authorId,
      publishedDate: book.publishedDate?.slice(0, 10) || today,
      copiesTotal: book.copiesTotal,
    });
    setOpen(true);
  }

  async function updateBook() {
    try {
      await api.put(`/books/${form.id}`, {
        title: form.title,
        isbn: form.isbn,
        authorId: Number(form.authorId),
        publishedDate: form.publishedDate || today,
        copiesTotal: Number(form.copiesTotal),
      });

      setOpen(false);
      loadBooks();
    } catch (err) {
      handleApiError(err);
    }
  }

  // -----------------------------------------------------------
  // Handle API Validation Errors
  // -----------------------------------------------------------
  function handleApiError(err) {
    console.log("Error response:", err);

    if (err.response?.data?.errors) {
      // ASP.NET Core validation dictionary
      const firstKey = Object.keys(err.response.data.errors)[0];
      setError(err.response.data.errors[firstKey][0]);
    } else if (err.response?.data?.message) {
      // Custom exceptions (NotFoundException, ValidationException, etc.)
      setError(err.response.data.message);
    } else {
      setError("An unexpected error occurred.");
    }
  }

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", mt: 4 }}>
      <Typography variant="h4" sx={{ mb: 3 }}>
        📘 Manage Books
      </Typography>

      <Button variant="contained" sx={{ mb: 2 }} onClick={openCreateModal}>
        Add New Book
      </Button>

      {books.map((b) => (
        <Card key={b.id} sx={{ mb: 2 }}>
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

      {/* ----------------- MODAL ------------------- */}
      <Dialog open={open} onClose={() => setOpen(false)} fullWidth>
        <DialogTitle>{editMode ? "Edit Book" : "Add Book"}</DialogTitle>

        <DialogContent>
          {error && (
            <Typography color="error" sx={{ mb: 2 }}>
              ⚠️ {error}
            </Typography>
          )}

          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="Title"
              name="title"
              value={form.title}
              onChange={handleChange}
              fullWidth
            />

            <TextField
              label="ISBN"
              name="isbn"
              value={form.isbn}
              onChange={handleChange}
              fullWidth
            />

            <TextField
              select
              label="Author"
              name="authorId"
              value={form.authorId}
              onChange={handleChange}
              fullWidth
            >
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
            />

            <TextField
              type="number"
              label="Copies Total"
              name="copiesTotal"
              value={form.copiesTotal}
              onChange={handleChange}
              fullWidth
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
