import { useEffect, useState } from "react";
import { Box, Grid, Paper, Typography, CircularProgress } from "@mui/material";
import api from "../api/AxiosClient";

export default function DashboardSummary({ version = 0 }) {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");

  useEffect(() => {
    let active = true;

    const loadDashboard = async () => {
      try {
        setLoadError("");

        if (!stats && version === 0) {
          setLoading(true);
        }

        const res = await api.get("/dashboard");
        if (!active) return;

        setStats(res.data);
        setLoading(false);
      } catch (err) {
        console.error("Failed to load dashboard", err);
        if (!active) return;
        setLoadError("Failed to load dashboard data.");
        setLoading(false);
      }
    };

    loadDashboard();
    return () => {
      active = false;
    };
  }, [version]);

  if (loading && !stats) {
    return (
      <Box sx={{ textAlign: "center", mt: 2 }}>
        <CircularProgress size={30} />
      </Box>
    );
  }

  if (loadError && !stats) {
    return (
      <Typography sx={{ mt: 2, textAlign: "center" }}>{loadError}</Typography>
    );
  }

  if (!stats) return null;

  const items = [
    { label: "Total Books", value: stats.totalBooks },
    { label: "Total Members", value: stats.totalMembers },
    { label: "Active Loans", value: stats.activeLoans },
    { label: "Overdue Loans", value: stats.overdueLoans },
  ];

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", mt: 4, mb: 5, textAlign: "center" }}>
      <Grid container spacing={2}>
        {items.map((item) => (
          <Grid item xs={6} sm={3} key={item.label}>
            <Paper
              elevation={1}
              sx={{
                p: 2,
                textAlign: "center",
                borderRadius: 4,
                backgroundColor: "white",
                boxShadow: "0 3px 8px rgba(0,0,0,0.10)",
                mx: "auto",
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
