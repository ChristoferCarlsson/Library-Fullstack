import { useEffect, useState } from "react";
import { Box, Grid, Paper, Typography, CircularProgress } from "@mui/material";
import api from "../api/AxiosClient";

export default function DashboardSummary() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadDashboard() {
      try {
        const res = await api.get("/dashboard");
        setStats(res.data);
      } catch (err) {
        console.error("Failed to load dashboard", err);
      }
      setLoading(false);
    }

    loadDashboard();
  }, []);

  if (loading) {
    return (
      <Box sx={{ textAlign: "center", mt: 2 }}>
        <CircularProgress size={30} />
      </Box>
    );
  }

  if (!stats) {
    return (
      <Typography sx={{ mt: 2, textAlign: "center" }}>
        Failed to load dashboard data.
      </Typography>
    );
  }

  const items = [
    { label: "Total Books", value: stats.totalBooks },
    { label: "Total Members", value: stats.totalMembers },
    { label: "Active Loans", value: stats.activeLoans },
    { label: "Overdue Loans", value: stats.overdueLoans },
  ];

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", mt: 2, mb: 4 }}>
      <Grid container spacing={2}>
        {items.map((item, idx) => (
          <Grid item xs={6} sm={3} key={idx}>
            <Paper
              elevation={2}
              sx={{
                p: 2,
                textAlign: "center",
                borderRadius: 2,
                backgroundColor: "#f0f4ff",
              }}
            >
              <Typography variant="h6" sx={{ fontWeight: "bold" }}>
                {item.value}
              </Typography>
              <Typography variant="body2">{item.label}</Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
