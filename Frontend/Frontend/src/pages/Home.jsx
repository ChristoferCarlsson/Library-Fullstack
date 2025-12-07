import { useState, useEffect } from "react";
import {
  Box,
  TextField,
  MenuItem,
  Button,
  Typography,
  Card,
  CardContent,
  Pagination,
  Stack,
} from "@mui/material";
import api from "../api/AxiosClient.js";

export default function Home() {
  const [mode, setMode] = useState("books"); // books | authors
  const [query, setQuery] = useState("");
  const [results, setResults] = useState([]);
  const [authors, setAuthors] = useState([]);

  const [selectedAuthor, setSelectedAuthor] = useState("");
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [totalPages, setTotalPages] = useState(1);

  // ---------------------------------------------------------
  // Load authors for dropdown (one-time on page load)
  // ---------------------------------------------------------
  useEffect(() => {
    async function loadAuthors() {
      try {
        const res = await api.get("/authors");
        setAuthors(res.data);
      } catch (err) {
        console.error("Failed loading authors", err);
      }
    }
    loadAuthors();
  }, []);

  // ---------------------------------------------------------
  // Reset pagination when query or mode changes
  // ---------------------------------------------------------
  useEffect(() => {
    setPage(1);
  }, [query, mode, selectedAuthor]);

  // ---------------------------------------------------------
  // Fetch All Books
  // ---------------------------------------------------------
  const loadAllBooks = async () => {
    setLoading(true);
    try {
      const res = await api.get("/books");
      setResults(res.data);
      setTotalPages(1);
    } catch (err) {
      console.error("Error loading all books", err);
    }
    setLoading(false);
  };

  // ---------------------------------------------------------
  // Search Handler (Books or Authors)
  // ---------------------------------------------------------
  const handleSearch = async () => {
    setLoading(true);
    setResults([]);

    // Case 1: empty query → Load all books
    if (mode === "books" && !query.trim() && !selectedAuthor) {
      await loadAllBooks();
      setLoading(false);
      return;
    }

    try {
      // -----------------------------------
      // BOOK SEARCH (server-side)
      // -----------------------------------
      if (mode === "books") {
        const res = await api.get("/books/search", {
          params: {
            Title: query || undefined,
            AuthorId: selectedAuthor || undefined,
            Page: page,
            PageSize: pageSize,
            SortBy: "Title",
            Desc: false,
          },
        });

        setResults(res.data.items);
        setTotalPages(res.data.totalPages);
      }

      // -----------------------------------
      // AUTHOR SEARCH (client-side)
      // -----------------------------------
      if (mode === "authors") {
        const res = await api.get("/authors");

        const filtered = res.data.filter((a) =>
          a.fullName.toLowerCase().includes(query.toLowerCase())
        );

        setResults(filtered);
        setTotalPages(1);
      }
    } catch (err) {
      console.error("Search error:", err);
    }

    setLoading(false);
  };

  // When page changes, re-run search
  useEffect(() => {
    if (mode === "books") {
      handleSearch();
    }
  }, [page]);

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", mt: 4 }}>
      <Typography variant="h4" sx={{ mb: 3, textAlign: "center" }}>
        📚 Library Search
      </Typography>

      {/* ----------------------- SEARCH BAR ----------------------- */}
      <Stack direction="row" spacing={2} sx={{ mb: 3 }}>
        <TextField
          label={mode === "books" ? "Search books…" : "Search authors…"}
          fullWidth
          variant="outlined"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />

        <TextField
          select
          label="Mode"
          value={mode}
          sx={{ width: 150 }}
          onChange={(e) => {
            setMode(e.target.value);
            setSelectedAuthor("");
          }}
        >
          <MenuItem value="books">Books</MenuItem>
          <MenuItem value="authors">Authors</MenuItem>
        </TextField>

        <Button
          variant="contained"
          color="primary"
          onClick={handleSearch}
          sx={{ minWidth: 120 }}
        >
          Search
        </Button>
      </Stack>

      {/* ----------------------- AUTHOR FILTER ----------------------- */}
      {mode === "books" && (
        <TextField
          select
          label="Filter by author"
          fullWidth
          value={selectedAuthor}
          onChange={(e) => setSelectedAuthor(e.target.value)}
          sx={{ mb: 3 }}
        >
          <MenuItem value="">All Authors</MenuItem>
          {authors.map((a) => (
            <MenuItem key={a.id} value={a.id}>
              {a.fullName}
            </MenuItem>
          ))}
        </TextField>
      )}

      {/* ----------------------- SHOW ALL BUTTON ----------------------- */}
      {mode === "books" && (
        <Button
          variant="outlined"
          fullWidth
          sx={{ mb: 2 }}
          onClick={loadAllBooks}
        >
          Show All Books
        </Button>
      )}

      {/* ----------------------- RESULTS ----------------------- */}
      {loading && <Typography>Loading...</Typography>}

      {!loading && results.length === 0 && (
        <Typography>No results found.</Typography>
      )}

      {!loading &&
        results.map((item) => (
          <Card key={item.id} sx={{ mb: 2 }}>
            <CardContent>
              {mode === "books" ? (
                <>
                  <Typography variant="h6">{item.title}</Typography>
                  <Typography>ISBN: {item.isbn}</Typography>
                  <Typography>Author: {item.authorFullName}</Typography>
                  <Typography>
                    Copies Available: {item.copiesAvailable}
                  </Typography>
                </>
              ) : (
                <>
                  <Typography variant="h6">{item.fullName}</Typography>
                  <Typography>{item.description}</Typography>
                </>
              )}
            </CardContent>
          </Card>
        ))}

      {/* ----------------------- PAGINATION ----------------------- */}
      {mode === "books" && totalPages > 1 && (
        <Stack alignItems="center" sx={{ mt: 3 }}>
          <Pagination
            count={totalPages}
            page={page}
            onChange={(e, val) => setPage(val)}
            color="primary"
          />
        </Stack>
      )}
    </Box>
  );
}
