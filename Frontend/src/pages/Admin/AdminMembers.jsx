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

export default function AdminMembers({ onDataChanged }) {
  const [members, setMembers] = useState([]);
  const [open, setOpen] = useState(false);
  const [editMode, setEditMode] = useState(false);

  const [apiError, setApiError] = useState("");
  const [fieldErrors, setFieldErrors] = useState({});

  const emptyForm = { id: null, firstName: "", lastName: "", email: "" };
  const [form, setForm] = useState(emptyForm);

  const notifyChanged = () => {
    if (typeof onDataChanged === "function") onDataChanged();
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setFieldErrors((prev) => ({ ...prev, [name]: "" }));
  };

  const resetModal = (edit = false, values = emptyForm) => {
    setEditMode(edit);
    setApiError("");
    setFieldErrors({});
    setForm(values);
    setOpen(true);
  };

  const loadMembers = async () => {
    const res = await api.get("/members");
    setMembers(res.data);
  };

  const handleApiError = (err) => {
    const resp = err.response?.data;

    if (resp?.errors) {
      const key = Object.keys(resp.errors)[0];
      setApiError(resp.errors[key][0]);
    } else {
      setApiError(resp?.message || "An unexpected error occurred.");
    }
  };

  useEffect(() => {
    loadMembers();
  }, []);

  const validateMemberForm = (form) => {
    const errors = validate(form, {
      firstName: { required: true, label: "first name" },
      lastName: { required: true, label: "last name" },
      email: { required: true, label: "email", email: true },
    });
    setFieldErrors(errors);
    return !hasErrors(errors);
  };

  const createMember = async () => {
    if (!validateMemberForm(form)) return;

    try {
      await api.post("/members", {
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: form.email.trim(),
      });

      setOpen(false);
      await loadMembers();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  const updateMember = async () => {
    if (!validateMemberForm(form)) return;

    try {
      await api.put(`/members/${form.id}`, {
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: form.email.trim(),
      });

      setOpen(false);
      await loadMembers();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  const openCreateModal = () => resetModal(false);
  const openEditModal = (member) => resetModal(true, member);

  const deleteMember = async (id) => {
    if (!window.confirm("Delete this member?")) return;
    setApiError("");

    try {
      await api.delete(`/members/${id}`);
      await loadMembers();
      notifyChanged();
    } catch (err) {
      handleApiError(err);
    }
  };

  return (
    <Box sx={{ width: "100%", maxWidth: 900, mx: "auto", mt: 3, mb: 4 }}>
      <Typography variant="h4" className="page-title">
        👤 Manage Members
      </Typography>

      <Button variant="contained" sx={{ mb: 2 }} onClick={openCreateModal}>
        Add New Member
      </Button>

      {apiError && (
        <Typography color="error" sx={{ mb: 2 }}>
          ⚠️ {apiError}
        </Typography>
      )}

      {members.map((m) => (
        <Card
          key={m.id}
          sx={{ mb: 2, "&:hover": { backgroundColor: "#faf7f2" } }}
        >
          <CardContent>
            <Typography variant="h6">
              {m.firstName} {m.lastName}
            </Typography>

            <Typography>Email: {m.email}</Typography>

            <Typography>
              Joined: {m.joinedAt ? m.joinedAt.slice(0, 10) : "Unknown"}
            </Typography>

            <Typography sx={{ mt: 1, fontWeight: "bold" }}>
              Loans: {m.loanCount ?? 0}
            </Typography>

            <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
              <Button variant="outlined" onClick={() => openEditModal(m)}>
                Edit
              </Button>
              <Button
                color="error"
                variant="outlined"
                onClick={() => deleteMember(m.id)}
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
        PaperProps={{
          sx: { borderRadius: 3, backgroundColor: "#fffdf8" },
        }}
      >
        <DialogTitle>{editMode ? "Edit Member" : "Add Member"}</DialogTitle>

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
              label="Email"
              name="email"
              value={form.email}
              onChange={handleChange}
              type="email"
              fullWidth
              error={!!fieldErrors.email}
              helperText={fieldErrors.email}
            />
          </Stack>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={editMode ? updateMember : createMember}
          >
            {editMode ? "Update" : "Create"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
