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
import { validate, hasErrors } from "../../utils/validate.js";

export default function AdminAuthors({ onDataChanged }) {
  const [authors, setAuthors] = useState([]);

  const [open, setOpen] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [apiError, setApiError] = useState("");
  const [fieldErrors, setFieldErrors] = useState({});

  const emptyForm = { id: null, firstName: "", lastName: "", description: "" };
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

  const loadAuthors = async () => {
    const res = await api.get("/authors");
    setAuthors(res.data);
  };

  useEffect(() => {
    loadAuthors();
  }, []);

  const validateAuthorForm = (data) => {
    const errors = validate(data, {
      firstName: { required: true, label: "first name" },
      lastName: { required: true, label: "last name" },
    });

    setFieldErrors(errors);
    return !hasErrors(errors);
  };

  const createAuthor = async () => {
    if (!validateAuthorForm(form)) return;

    try {
      await api.post("/authors", {
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        description: form.description.trim() || null,
      });

      setOpen(false);
      await loadAuthors();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  const updateAuthor = async () => {
    if (!validateAuthorForm(form)) return;

    try {
      await api.put(`/authors/${form.id}`, {
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        description: form.description.trim() || null,
      });

      setOpen(false);
      await loadAuthors();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  const openCreateModal = () => resetModal(false);
  const openEditModal = (author) =>
    resetModal(true, {
      id: author.id,
      firstName: author.firstName,
      lastName: author.lastName,
      description: author.description || "",
    });

  const deleteAuthor = async (id) => {
    if (!window.confirm("Delete this author? This may affect books!")) return;

    setApiError("");

    try {
      await api.delete(`/authors/${id}`);
      await loadAuthors();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  return (
    <Box sx={{ width: "100%", maxWidth: 900, mx: "auto", mt: 3, mb: 4 }}>
      <Typography variant="h4" className="page-title">
        ✍️ Manage Authors
      </Typography>

      {apiError && (
        <Typography color="error" sx={{ mb: 2 }}>
          ⚠️ {apiError}
        </Typography>
      )}

      <Button variant="contained" sx={{ mb: 2 }} onClick={openCreateModal}>
        Add New Author
      </Button>

      {authors.map((a) => (
        <Card
          key={a.id}
          sx={{ mb: 2, "&:hover": { backgroundColor: "#faf7f2" } }}
        >
          <CardContent>
            <Typography variant="h6">{a.fullName}</Typography>

            <Typography sx={{ whiteSpace: "pre-line" }}>
              {a.description}
            </Typography>

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

      {/* Modal */}
      <Dialog
        open={open}
        onClose={() => setOpen(false)}
        fullWidth
        PaperProps={{ sx: { borderRadius: 3, backgroundColor: "#fffdf8" } }}
      >
        <DialogTitle>{editMode ? "Edit Author" : "Add Author"}</DialogTitle>

        <DialogContent>
          {apiError && (
            <Typography color="error" sx={{ mb: 2 }}>
              ⚠️ {apiError}
            </Typography>
          )}

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
