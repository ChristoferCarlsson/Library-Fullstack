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
} from "@mui/material";
import api from "../../api/AxiosClient";

export default function AdminAuthors() {
  const [authors, setAuthors] = useState([]);

  const [open, setOpen] = useState(false);
  const [editMode, setEditMode] = useState(false);

  const [error, setError] = useState("");

  const [form, setForm] = useState({
    id: null,
    firstName: "",
    lastName: "",
    description: "",
  });

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function loadAuthors() {
    const res = await api.get("/authors");
    setAuthors(res.data);
  }

  // --------------------------------------------------------
  // DELETE AUTHOR (with error handling)
  // --------------------------------------------------------
  async function deleteAuthor(id) {
    if (!window.confirm("Delete this author? This may affect books!")) return;

    setError("");

    try {
      await api.delete(`/authors/${id}`);
      loadAuthors();
    } catch (err) {
      handleApiError(err);
    }
  }

  useEffect(() => {
    loadAuthors();
  }, []);

  // --------------------------------------------------------
  // OPEN CREATE MODAL
  // --------------------------------------------------------
  function openCreateModal() {
    setEditMode(false);
    setError("");

    setForm({
      id: null,
      firstName: "",
      lastName: "",
      description: "",
    });

    setOpen(true);
  }

  async function createAuthor() {
    try {
      await api.post("/authors", {
        firstName: form.firstName,
        lastName: form.lastName,
        description: form.description,
      });

      setOpen(false);
      loadAuthors();
    } catch (err) {
      handleApiError(err);
    }
  }

  // --------------------------------------------------------
  // EDIT AUTHOR
  // --------------------------------------------------------
  function openEditModal(author) {
    setEditMode(true);
    setError("");

    setForm({
      id: author.id,
      firstName: author.firstName,
      lastName: author.lastName,
      description: author.description || "",
    });

    setOpen(true);
  }

  async function updateAuthor() {
    try {
      await api.put(`/authors/${form.id}`, {
        firstName: form.firstName,
        lastName: form.lastName,
        description: form.description,
      });

      setOpen(false);
      loadAuthors();
    } catch (err) {
      handleApiError(err);
    }
  }

  // --------------------------------------------------------
  // Handle API Validation Errors
  // --------------------------------------------------------
  function handleApiError(err) {
    if (err.response?.data?.errors) {
      const firstKey = Object.keys(err.response.data.errors)[0];
      setError(err.response.data.errors[firstKey][0]);
    } else if (err.response?.data?.message) {
      setError(err.response.data.message);
    } else {
      setError("An unexpected error occurred.");
    }
  }

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", mt: 4 }}>
      <Typography variant="h4" sx={{ mb: 3 }}>
        ✍️ Manage Authors
      </Typography>

      {/* Global Page Error */}
      {error && (
        <Typography color="error" sx={{ mb: 2 }}>
          ⚠️ {error}
        </Typography>
      )}

      <Button variant="contained" sx={{ mb: 2 }} onClick={openCreateModal}>
        Add New Author
      </Button>

      {authors.map((a) => (
        <Card key={a.id} sx={{ mb: 2 }}>
          <CardContent>
            <Typography variant="h6">{a.fullName}</Typography>
            <Typography>{a.description}</Typography>

            <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
              <Button variant="outlined" onClick={() => openEditModal(a)}>
                Edit
              </Button>

              <Button
                color="error"
                variant="outlined"
                onClick={() => deleteAuthor(a.id)}
              >
                Delete
              </Button>
            </Stack>
          </CardContent>
        </Card>
      ))}

      {/* ----------------------- MODAL ------------------------ */}
      <Dialog open={open} onClose={() => setOpen(false)} fullWidth>
        <DialogTitle>{editMode ? "Edit Author" : "Add Author"}</DialogTitle>

        <DialogContent>
          {error && (
            <Typography color="error" sx={{ mb: 2 }}>
              ⚠️ {error}
            </Typography>
          )}

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
              label="Description"
              name="description"
              multiline
              rows={3}
              value={form.description}
              onChange={handleChange}
              fullWidth
            />
          </Stack>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>

          <Button
            variant="contained"
            onClick={editMode ? updateAuthor : createAuthor}
          >
            {editMode ? "Update" : "Create"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
