CREATE TABLE Categories(
categoryId varchar(50) primary key,
categoryName varchar(255) not null
);

CREATE TABLE Tickets(
ticketCode VARCHAR(50) PRIMARY KEY,
ticketName VARCHAR(255) NOT NULL,
categoryId VARCHAR(50) NOT NULL,
price INT NOT NULL,
quota INT NOT NULL,
eventDate DATETIME DEFAULT GETDATE(),
FOREIGN KEY(categoryId) REFERENCES Categories(categoryId) ON DELETE CASCADE,
);

CREATE TABLE BookedTicket(
id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
bookedTicketId VARCHAR(50) NOT NULL,
ticketCode VARCHAR(50),
quantity INT NOT NULL,
FOREIGN KEY (ticketCode) REFERENCES Tickets(ticketCode),
CONSTRAINT Unique_BookedTicketId_TicketCode UNIQUE (bookedTicketId, ticketCode)
);

INSERT INTO Categories (categoryId, categoryName) VALUES
(0, 'Cinema'), 
(1, 'Hotel'), 
(2, 'Transportasi Darat'), 
(3, 'Transportasi Laut');

INSERT INTO Tickets (ticketCode, ticketName, categoryId, price, quota, eventDate) VALUES 
('TD001', 'Bus Jawa-Sumatera', 2, 50000, 80, '2022-03-01'),
('TL001', 'Kapal Ferri Jawa-Sumatera', 3, 50000, 70, '2022-03-01'),
('C001', 'Ironman CGV', 0, 50000, 99, '2022-03-01'),
('H001', 'Ibis Hotel Jakarta 21-23', 1, 50000, 76, '2022-03-01');

INSERT INTO BookedTicket (bookedTicketId, ticketCode, quantity) VALUES
('B001', 'C001', 6),
('B001', 'H001', 4),
('B001', 'TD001', 10);
