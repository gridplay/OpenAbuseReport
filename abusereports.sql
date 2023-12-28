CREATE TABLE IF NOT EXISTS `abusereports` (
  `id` varchar(255) DEFAULT NULL,
  `reporter` varchar(255) DEFAULT NULL,
  `abuser` varchar(255) DEFAULT NULL,
  `img` varchar(255) DEFAULT NULL,
  `summary` varchar(255) DEFAULT NULL,
  `details` longtext DEFAULT NULL,
  `posted` int(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
