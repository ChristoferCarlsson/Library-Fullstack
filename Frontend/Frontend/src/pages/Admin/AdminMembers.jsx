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

export default function AdminMembers() {
  const [members, setMembers] = useState([]);

  const [open, setOpen] = useState(false);
  const [editMode, setEditMode] = useState(false);

  const [error, setError] = useState("");

  const [form, setForm] = useState({
    id: null,
    firstName: "",
    lastName: "",
    email: "",
  });

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function loadMembers() {
    const res = await api.get("/members");
    setMembers(res.data);
  }

  async function deleteMember(id) {
    if (!window.confirm("Delete this member?")) return;
    await api.delete(`/members/${id}`);
    loadMembers();
  }

  useEffect(() => {
    loadMembers();
  }, []);

  // ---------------------------------------------------------
  // CREATE MEMBER
  // ---------------------------------------------------------
  function openCreateModal() {
    setEditMode(false);
    setError("");
    setForm({
      id: null,
      firstName: "",
      lastName: "",
      email: "",
    });
    setOpen(true);
  }

  async function createMember() {
    try {
      await api.post("/members", {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
      });

      setOpen(false);
      loadMembers();
    } catch (err) {
      handleApiError(err);
    }
  }

  // ---------------------------------------------------------
  // EDIT MEMBER
  // ---------------------------------------------------------
  function openEditModal(member) {
    setEditMode(true);
    setError("");
    setForm({
      id: member.id,
      firstName: member.firstName,
      lastName: member.lastName,
      email: member.email,
    });
    setOpen(true);
  }

  async function updateMember() {
    try {
      await api.put(`/members/${form.id}`, {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
      });

      setOpen(false);
      loadMembers();
    } catch (err) {
      handleApiError(err);
    }
  }

  // ---------------------------------------------------------
  // Extract Backend Errors
  // ---------------------------------------------------------
  function handleApiError(err) {
    if (err.response?.data?.errors) {
      // ASP.NET validation dictionary → pick the first error message
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
        👤 Manage Members
      </Typography>

      <Button variant="contained" sx={{ mb: 2 }} onClick={openCreateModal}>
        Add New Member
      </Button>

      {members.map((m) => (
        <Card key={m.id} sx={{ mb: 2 }}>
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

      {/* ----------------------------------------------------------
          MEMBER FORM MODAL
      ----------------------------------------------------------- */}
      <Dialog open={open} onClose={() => setOpen(false)} fullWidth>
        <DialogTitle>{editMode ? "Edit Member" : "Add Member"}</DialogTitle>

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
              label="Email"
              name="email"
              value={form.email}
              onChange={handleChange}
              type="email"
              fullWidth
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
