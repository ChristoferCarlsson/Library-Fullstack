import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
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
  const navigate = useNavigate();

  const [mode, setMode] = useState("books");
  const [query, setQuery] = useState("");
  const [results, setResults] = useState([]);
  const [authors, setAuthors] = useState([]);

  const [selectedAuthor, setSelectedAuthor] = useState("");
  const [loading, setLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [totalPages, setTotalPages] = useState(1);

  // Reset to page 1 whenever filters change
  useEffect(() => {
    setPage(1);
  }, [query, mode, selectedAuthor]);

  const loadAuthorsIfNeeded = async () => {
    if (authors.length) return;
    try {
      const res = await api.get("/authors");
      setAuthors(res.data);
    } catch (err) {
      console.error("Failed to load authors", err);
    }
  };

  const loadAllBooks = async () => {
    try {
      setLoading(true);
      setHasSearched(true);
      const res = await api.get("/books");
      setResults(res.data);
      setTotalPages(1);
    } catch (err) {
      console.error("Error loading books", err);
    } finally {
      setLoading(false);
    }
  };

  const loadAllAuthors = async () => {
    try {
      setLoading(true);
      setHasSearched(true);
      const res = await api.get("/authors");
      setResults(res.data);
      setTotalPages(1);
    } catch (err) {
      console.error("Error loading authors", err);
    } finally {
      setLoading(false);
    }
  };

  const searchBooks = async () => {
    await loadAuthorsIfNeeded();

    if (!query.trim() && !selectedAuthor) {
      return loadAllBooks();
    }

    const params = {
      Title: query || undefined,
      AuthorId: selectedAuthor || undefined,
      Page: page,
      PageSize: pageSize,
      SortBy: "Title",
      Desc: false,
    };

    const res = await api.get("/books/search", { params });
    setResults(res.data.items);
    setTotalPages(res.data.totalPages);
  };

  const searchAuthors = async () => {
    const res = await api.get("/authors");
    const filtered = res.data.filter((a) =>
      a.fullName.toLowerCase().includes(query.toLowerCase())
    );
    setResults(filtered);
    setTotalPages(1);
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      setHasSearched(true);

      if (mode === "books") await searchBooks();
      else await searchAuthors();
    } catch (err) {
      console.error("Search error:", err);
    } finally {
      setLoading(false);
    }
  };

  // When page changes (books only), re-search
  useEffect(() => {
    if (mode === "books" && hasSearched) {
      handleSearch();
    }
  }, [page]);

  return (
    <Box sx={{ width: "100%", maxWidth: 800, mx: "auto", mt: 3, mb: 10 }}>
      <Typography variant="h4" className="page-title">
        Search The Library
      </Typography>

      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={2}
        sx={{ mb: 3, alignItems: { xs: "stretch", sm: "center" } }}
      >
        <TextField
          label={mode === "books" ? "Search books…" : "Search authors…"}
          fullWidth
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />

        <TextField
          select
          label="Mode"
          value={mode}
          sx={{ width: 150 }}
          onChange={async (e) => {
            const newMode = e.target.value;
            setMode(newMode);
            setResults([]);
            setHasSearched(false);
            setSelectedAuthor("");

            if (newMode === "books") await loadAuthorsIfNeeded();
          }}
        >
          <MenuItem value="books">Books</MenuItem>
          <MenuItem value="authors">Authors</MenuItem>
        </TextField>

        <Button
          variant="contained"
          onClick={handleSearch}
          sx={{ minWidth: 120 }}
        >
          Search
        </Button>
      </Stack>

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

      <Button
        variant="outlined"
        fullWidth
        sx={{ mb: 2 }}
        onClick={mode === "books" ? loadAllBooks : loadAllAuthors}
      >
        {mode === "books" ? "Show All Books" : "Show All Authors"}
      </Button>

      {loading && <Typography>Loading...</Typography>}

      {!loading && hasSearched && results.length === 0 && (
        <Typography>No results found.</Typography>
      )}

      {!loading &&
        results.map((item) => (
          <Card
            key={item.id}
            sx={{ mb: 2, cursor: mode === "books" ? "pointer" : "default" }}
            onClick={() =>
              mode === "books" ? navigate(`/books/${item.id}`) : undefined
            }
          >
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

      {mode === "books" && totalPages > 1 && (
        <Stack alignItems="center" sx={{ mt: 3 }}>
          <Pagination
            count={totalPages}
            page={page}
            onChange={(e, val) => setPage(val)}
          />
        </Stack>
      )}
    </Box>
  );
}
