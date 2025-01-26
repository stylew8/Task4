import React, { useState, useEffect } from "react";
import { DataGrid } from "@mui/x-data-grid";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import TextField from "@mui/material/TextField";
import { getCookie, deleteCookie } from "../utils/session";
import Constants from "../utils/constants";
import { deleteWithAuth, patchWithAuth, postWithAuth, getWithAuth } from "../utils/api";

const UserTable = () => {
  const [allRows, setAllRows] = useState([]);
  const [filteredRows, setFilteredRows] = useState([]); 
  const [selectedUsers, setSelectedUsers] = useState([]); 
  const [searchText, setSearchText] = useState(""); 
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    page: 0,
    pageSize: 10,
    total: 0,
  });

  const columns = [
    { field: "fullName", headerName: "Full Name", flex: 1 },
    { field: "email", headerName: "Email", flex: 1 },
    { field: "lastSeen", headerName: "Last seen", flex: 1 },
    {
      field: "status",
      headerName: "Status",
      renderCell: (params) => (
        <span style={{ color: params.value === "Blocked" ? "red" : "green" }}>
          {params.value}
        </span>
      ),
    },
  ];

  const fetchUsers = async (page, pageSize, search) => {
    setLoading(true);
    try {
      const response = await getWithAuth("user/users", {
        params: {
          pageNumber: page + 1,
          pageSize,
          searchText: search,
        },
      }, getCookie("SessionId"));
      const { users, totalCount } = response.data;
      setAllRows(users.map((user) => ({ ...user, id: user.id }))); 
      setFilteredRows(users.map((user) => ({ ...user, id: user.id })));
      setPagination((prev) => ({ ...prev, total: totalCount }));
    } catch (error) {
      console.error("Error fetching users:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers(pagination.page, pagination.pageSize, searchText);
  }, []);

  const handleSelection = (newSelectionModel) => {
    setSelectedUsers(newSelectionModel);
  };

  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchText(value);
  
    const filtered = allRows.filter((row) => {
      const name = row.name || "";
      const email = row.email || "";
      const status = row.status || "";
  
      return (
        name.toLowerCase().includes(value) ||
        email.toLowerCase().includes(value) ||
        status.toLowerCase().includes(value)
      );
    });
  
    setFilteredRows(filtered);
  };

  const handleLogout = async () => {
    const sessionId = getCookie("SessionId");
    const rememberMe = getCookie("RememberMe");

    try {
      await deleteWithAuth("auth/logout", { sessionId, rememberMe }, sessionId);
      deleteCookie("SessionId");
      deleteCookie("RememberMe");
      window.location.reload();
    } catch (error) {
      console.error("Error logging out:", error);
    }
  };

  const handleBlock = async () => {
    const sessionGuid = getCookie("SessionId");
  
    try {
      const response = await postWithAuth(
        "user/users", 
        { usersId: selectedUsers }, 
        sessionGuid 
      );

      const updatedRows = allRows.map((row) =>
        selectedUsers.includes(row.id)
          ? { ...row, status: "Blocked" } 
          : row
      );
  
      setAllRows(updatedRows); 
      setFilteredRows(updatedRows);
      setSelectedUsers([]);
    } catch (error) {
      console.error("Error blocking users:", error);
    }
  };
  
  const handleUnblock = async () => {
    const sessionGuid = getCookie("SessionId"); 
  
    try {
      const response = await patchWithAuth(
        "user/users", 
        { usersId: selectedUsers }, 
        sessionGuid 
      );
      const updatedRows = allRows.map((row) =>
        selectedUsers.includes(row.id)
          ? { ...row, status: "Active" } 
          : row
      );
  
      setAllRows(updatedRows); 
      setFilteredRows(updatedRows); 
      setSelectedUsers([]);
    } catch (error) {
      console.error("Error unblocking users:", error);
    }
  };
  
  const handleDelete = async () => {
    const sessionGuid = getCookie("SessionId"); 
  
    try {
      const response = await deleteWithAuth(
        "user/users", 
        { usersId: selectedUsers }, 
        sessionGuid 
      );

      const updatedRows = allRows.filter((row) => !selectedUsers.includes(row.id));

      setAllRows(updatedRows); 
      setFilteredRows(updatedRows); 
      setSelectedUsers([]);
    } catch (error) {
      console.error("Error deleting users:", error);
    }
  };

  return (
    <Box
      sx={{
        height: "auto",
        width: "100%",
        bgcolor: "#f9f9f9",
        borderRadius: 2,
        boxShadow: 1,
      }}
    >
      <div
        style={{
          display: "flex",
          justifyContent: "flex-end",
          marginBottom: "5px",
          padding: "2px",
        }}
      >
        <Button color="error" onClick={handleLogout}>
          Logout
        </Button>
      </div>
      <Box sx={{ display: "flex", justifyContent: "space-between", p: "2px" }}>
        <Box>
          <Button
            variant="contained"
            color="primary"
            sx={{ mr: 1 }}
            disabled={selectedUsers.length === 0}
            onClick={handleBlock}
          >
            Block
          </Button>
          <Button
            variant="contained"
            color="success"
            sx={{ mr: 1 }}
            disabled={selectedUsers.length === 0}
            onClick={handleUnblock}
          >
            Unblock
          </Button>
          <Button
            variant="contained"
            color="error"
            disabled={selectedUsers.length === 0}
            onClick={handleDelete}
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
        rows={filteredRows.slice(
          pagination.page * pagination.pageSize,
          pagination.page * pagination.pageSize + pagination.pageSize
        )} 
        columns={columns}
        checkboxSelection
        loading={loading}
        onRowSelectionModelChange={(newSelectionModel) =>
          handleSelection(newSelectionModel)
        }
        rowSelectionModel={selectedUsers}
        paginationMode="server"
        rowCount={filteredRows.length}
        pageSize={pagination.pageSize}
        onPageChange={(newPage) =>
          setPagination((prev) => ({ ...prev, page: newPage }))
        }
        onPageSizeChange={(newPageSize) =>
          setPagination((prev) => ({ ...prev, pageSize: newPageSize }))
        }
        page={pagination.page}
      />
    </Box>
  );
};

export default UserTable;
