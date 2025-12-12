export function validate(values, schema) {
  const errors = {};

  for (const field of Object.keys(schema)) {
    const config = schema[field];
    const raw = values[field];
    const value = typeof raw === "string" ? raw.trim() : raw ?? "";

    // Required
    if (
      config.required &&
      (value === "" || value === null || value === undefined)
    ) {
      errors[field] =
        config.requiredMessage ||
        (config.label
          ? `Please enter ${config.label.toLowerCase()}`
          : "This field is required");
      continue;
    }

    // Email
    if (config.email && value) {
      const emailPattern = /^\S+@\S+\.\S+$/;
      if (!emailPattern.test(value)) {
        errors[field] =
          config.emailMessage || "Please enter a valid email address";
        continue;
      }
    }

    // Minimum numeric value
    if (
      config.min != null &&
      value !== "" &&
      !Number.isNaN(Number(value)) &&
      Number(value) < config.min
    ) {
      errors[field] =
        config.minMessage || `Value must be at least ${config.min}`;
      continue;
    }

    // Custom validator
    if (typeof config.custom === "function") {
      const msg = config.custom(value, values);
      if (msg) errors[field] = msg;
    }
  }

  return errors;
}

export function hasErrors(errors) {
  return Object.keys(errors).length > 0;
}
