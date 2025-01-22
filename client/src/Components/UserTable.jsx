import React, { useState } from "react";
import { DataGrid } from "@mui/x-data-grid";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import TextField from "@mui/material/TextField";

// SWR

const UserTable = () => {

    const rows = [
        {
          id: 1,
          name: "Clare, Alex",
          email: "a_clare42@gmail.com",
          lastSeen: "5 minutes ago",
          status: "active",
        },
        {
          id: 2,
          name: "Morrison, Jim",
          email: "dmtimer9@dealyaari.com",
          lastSeen: "less than a minute ago",
          status: "active",
        },
        {
          id: 3,
          name: "Simone, Nina",
          email: "marishabelin@giftcode-ao.com",
          lastSeen: "3 weeks ago",
          status: "blocked",
        },
        {
          id: 4,
          name: "Zappa, Frank",
          email: "zappa_f@citybank.com",
          lastSeen: "less than a minute ago",
          status: "blocked",
        },
      ];

  const [selectedUsers, setSelectedUsers] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [filteredRows, setFilteredRows] = useState(rows);

  const columns = [
    {
      field: "name",
      headerName: "Name",
      flex: 1,
    },
    {
      field: "email",
      headerName: "Email",
      flex: 1,
    },
    {
      field: "lastSeen",
      headerName: "Last seen",
      flex: 1,
    },
    {
      field: "status",
      headerName: "Status",
      flex: 1,
      renderCell: (params) => (
        <span style={{ color: params.value === "blocked" ? "red" : "green" }}>
          {params.value}
        </span>
      ),
    },
  ];



  const handleSelection = (newSelectionModel) => {
    setSelectedUsers(newSelectionModel);
  };

  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchText(value);
    const filtered = rows.filter(
      (row) =>
        row.name.toLowerCase().includes(value) ||
        row.email.toLowerCase().includes(value) ||
        row.status.toLowerCase().includes(value)
    );
    setFilteredRows(filtered);
  };

  return (
    <Box sx={{ height: "auto", width: "100%", p: 3, bgcolor: "#f9f9f9", borderRadius: 2, boxShadow: 1 }}>
      <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
        <Box>
          <Button
            variant="contained"
            color="primary"
            sx={{ mr: 1 }}
            disabled={selectedUsers.length === 0}
            onClick={() => alert("Blocked: " + selectedUsers.join(", "))}
          >
            Block
          </Button>
          <Button
            variant="contained"
            color="success"
            sx={{ mr: 1 }}
            disabled={selectedUsers.length === 0}
            onClick={() => alert("Unblocked: " + selectedUsers.join(", "))}
          >
            Unblock
          </Button>
          <Button
            variant="contained"
            color="error"
            disabled={selectedUsers.length === 0}
            onClick={() => alert("Deleted: " + selectedUsers.join(", "))}
          >
            Delete
          </Button>
        </Box>
        <TextField
          variant="outlined"
          placeholder="Search by name, email, or status"
          value={searchText}
          onChange={handleSearch}
          size="small"
          sx={{ width: "300px" }}
        />
      </Box>
      <DataGrid
        rows={filteredRows}
        columns={columns}
        checkboxSelection
        onRowSelectionModelChange={(newSelectionModel) => handleSelection(newSelectionModel)}
        rowSelectionModel={selectedUsers}
      />
    </Box>
  );
};

export default UserTable;
